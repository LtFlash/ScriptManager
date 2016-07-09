﻿using System;
using System.Collections.Generic;
using Rage;

namespace ScriptManager.Scripts
{
    public abstract class BaseScript : IBaseScript
    {
        //PUBLIC
        public bool HasFinished { get; private set; } = false;
        public bool Completed { get; private set; } = false;
        public bool IsRunning { get; private set; } = false;

        //PRIVATE
        private GameFiber _process;
        private bool _canRun = true;
        private List<Stage> _stages = new List<Stage>();
        private class Stage
        {
            public Action Function;
            public bool Active;

            public Stage(Action act, bool active)
            {
                Function = act;
                Active = active;
            }
        }

        public BaseScript()
        {
            //empty, ctor called to check CanBeStarted()
        }

        public abstract bool CanBeStarted();

        public void Start()
        {
            _process = new GameFiber(InternalProcess);
            RegisterStages();
            ActivateStage(InternalInitialize);
            _process.Start();
            IsRunning = true;
        }

        private void RegisterStages()
        {
            AddStage(InternalInitialize);
            AddStage(Process);
            AddStage(InternalEnd);
        }

        public void AddStage(Action stage)
        {
            _stages.Add(new Stage(stage, false));
        }

        public void ActivateStage(Action stage)
        {
            _stages.Find(a => a.Function == stage).Active = true;
        }

        public void DeactivateStage(Action stage)
        {
            _stages.Find(a => a.Function == stage).Active = false;
        }

        public void SwapStages(Action toDisable, Action toEnable)
        {
            DeactivateStage(toDisable);
            ActivateStage(toEnable);
        }

        private void InternalProcess()
        {
            while(_canRun)
            {
                ExecStages();
                GameFiber.Yield();
            }
        }

        private void ExecStages()
        {
            for (int i = 0; i < _stages.Count; i++)
            {
                if (_stages[i].Active) _stages[i].Function();
            }
        }

        protected void SetScriptFinished(bool completed)
        {
            Completed = completed;
            InternalEnd();

            //Abort() has to be the last as it does not return control to function!
            _process.Abort(); 
        }

        private void InternalInitialize()
        {
            Initialize();
            SwapStages(Initialize, Process);
        }

        public abstract void Initialize();

        public abstract void Process();

        private void InternalEnd()
        {
            _canRun = false;
            HasFinished = true;
            IsRunning = false;

            End();
        }

        public abstract void End();
    }
}
