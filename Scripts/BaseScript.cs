using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace ScriptManager.Scripts
{
    internal abstract class BaseScript : IBaseScript
    {
        //PUBLIC
        public bool HasFinished { get; private set; } = false;
        public bool Completed { get; private set; } = false;
        public bool IsRunning { get; private set; } = false;

        //PRIVATE
        private GameFiber _process;
        private bool _canRun;
        private List<Stage> _stages = new List<Stage>();
        private class Stage
        {
            public Action Act;
            public bool Active;
            public Stage(Action act, bool active)
            {
                Act = act;
                Active = active;
            }
        }

        public BaseScript()
        {
            _process = new GameFiber(InternalProcess);

            RegisterStages();
        }

        private void RegisterStages()
        {
            AddStage(Initialize);
            AddStage(Process);
            AddStage(End);

            ActivateStage(Initialize);
        }

        public void AddStage(Action stage)
        {
            _stages.Add(new Stage(stage, false));
        }

        public void ActivateStage(Action stage)
        {
            _stages.Find(a => a.Act == stage).Active = true;
        }

        public void DeactivateStage(Action stage)
        {
            _stages.Find(a => a.Act == stage).Active = false;
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
            }
        }

        private void ExecStages()
        {
            for (int i = 0; i < _stages.Count; i++)
            {
                if (_stages[i].Active) _stages[i].Act();
            }
        }

        protected void SetScriptFinished(bool completed)
        {
            _canRun = false;
            _process.Abort();
            Completed = completed;
            HasFinished = true;
            End();
        }

        public void Start()
        {
            _process.Start();
            IsRunning = true;
        }

        public virtual void Initialize()
        {
            SwapStages(Initialize, Process);
        }

        public virtual void Process()
        {

        }

        public virtual void End()
        {
            _process.Abort();
            HasFinished = true;
            IsRunning = false;
        }
    }
}
