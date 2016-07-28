﻿using System;
using System.Collections.Generic;
using Rage;

namespace ScriptManager.Scripts
{
    public abstract class BaseScript
    {
        //PUBLIC
        public bool HasFinished { get; private set; } = false;
        public bool Completed { get; protected set; } = false;
        public bool IsRunning { get; private set; } = false;

        //PROTECTED
        protected virtual ScriptStartController.IScriptStartController ScriptStartController { get; } = new ScriptStartController.UnconditionalStartController();

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

        public bool CanBeStarted()
        {
            return ScriptStartController.CanBeStarted();
        }

        public void Start()
        {
            _process = new GameFiber(InternalProcess);
            _process.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            _canRun = false;
            HasFinished = true;
            IsRunning = false;

            //Abort() has to be the last as it does not return control to function!
            _process.Abort();
        }

        protected void AddStage(Action stage)
        {
            _stages.Add(new Stage(stage, false));
        }

        protected void ActivateStage(Action stage)
        {
            _stages.Find(a => a.Function == stage).Active = true;
        }

        protected void DeactivateStage(Action stage)
        {
            _stages.Find(a => a.Function == stage).Active = false;
        }

        protected void SwapStages(Action toDisable, Action toEnable)
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

        protected abstract bool Initialize();
        protected abstract void Process();
        protected abstract void End();
    }
}
