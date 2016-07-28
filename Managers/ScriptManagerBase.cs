using System.Collections.Generic;
using System.Linq;
using Rage;
using System;

namespace ScriptManager.Managers
{
    public abstract class ScriptManagerBase : IScriptManager
    {
        //PROTECTED
        protected ScriptStatus _scriptToRunInFiber { get; set; }
        protected List<ScriptStatus> _scripts = new List<ScriptStatus>();

        //PRIVATE
        private GameFiber _process;
        private bool _canRun = true;

        public class ScriptStatus
        {
            public string Id { get; private set; }
            public Type TypeOfBaseScript { get; private set; }
            public Scripts.IScript Script { get; private set; }
            public string NextScriptToRunId { get; private set; }
            public bool Processed { get; set; } = false;

            public bool FinishedSuccessfully
            {
                get
                {
                    return Script != null && Script.Completed;
                }
            }

            public bool FinishedUnsuccessfully
            {
                get
                {
                    return Script != null && Script.HasFinished && !Script.Completed;

                }
            }

            public bool IsRunning
            {
                get
                {
                    return Script != null && Script.IsRunning;
                }
            }

            public void Start(bool checkIfCanBeStarted)
            {
                Scripts.IScript scriptToStart = CreateInstance(TypeOfBaseScript);
 
                if(!checkIfCanBeStarted || scriptToStart.CanBeStarted())
                {
                    Script = scriptToStart;
                    Script.Start();
                }
            }

            private Scripts.IScript CreateInstance(Type type)
            {
                return (Scripts.IScript)Activator.CreateInstance(type);
            }

            public ScriptStatus(string id, Type typeOfBaseScript, string nextScriptToRunId = "")
            {
                Id = id;
                TypeOfBaseScript = typeOfBaseScript;
                NextScriptToRunId = nextScriptToRunId;
            }
        }

        public ScriptManagerBase()
        {
            _process = new GameFiber(InternalProcess);
            _process.Start();
        }

        public ScriptStatus this[string id]
        {
            get
            {
                return _scripts.Find(s => s.Id == id);
            }
        }

        public void AddScript(string id, Type type)
        {
            if (type.GetInterfaces().Contains(typeof(Scripts.IScript)))
            {
                _scripts.Add(new ScriptStatus(id, type, ""));
            }
        }

        public void StartScript(string id, bool checkIfCanBeStarted)
        {
            ScriptStatus ss = _scripts.FirstOrDefault(s => s.Id == id);
            if (ss != null) ss.Start(checkIfCanBeStarted);
        }

        //public void StopScript(string id)
        //{
        //    ScriptStatus ss = _scripts.FirstOrDefault(s => s.Id == id);
        //    if (ss != null) ss.Script.End();
        //}

        private void InternalProcess()
        {
            while(_canRun)
            {
                //is being started from inside a GameFiber because of issues
                //with initialization from outside
                if(_scriptToRunInFiber != null)
                {
                    _scriptToRunInFiber.Start(true);
                    _scriptToRunInFiber = null;
                }

                Process();

                GameFiber.Yield();
            }
        }

        protected virtual void Process()
        {

        }
    }
}
