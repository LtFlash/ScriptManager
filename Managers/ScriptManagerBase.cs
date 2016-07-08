using System.Collections.Generic;
using System.Linq;
using Rage;
using System;

namespace ScriptManager.Managers
{
    public abstract class ScriptManagerBase
    {
        //PRIVATE
        private GameFiber _process;
        protected List<ScriptStatus> _scripts = new List<ScriptStatus>();
        private int _currentScript = 0;
        protected class ScriptStatus
        {
            public string Id;
            public Scripts.IBaseScript Script;
            public string NextScriptToRunId;
            public bool Processed = false;

            public ScriptStatus(string id, Scripts.IBaseScript script, string nextScriptToRunId = "")
            {
                Id = id;
                Script = script;
                NextScriptToRunId = nextScriptToRunId;
            }
        }
        private bool _canRun = true;

        public ScriptManagerBase()
        {
            _process = new GameFiber(InternalProcess);
            _process.Start();
        }

        //public void StartScriptManager()
        //{
        //}
        //public void AddScript(string id, Type type, bool start = false)
        //{
        //    Scripts.IBaseScript script = (Scripts.IBaseScript)Activator.CreateInstance(type);
        //}

        public void AddScript(string id, Scripts.IBaseScript script, bool start = false, string nextScriptToRunId = "")
        {
            _scripts.Add(new ScriptStatus(id, script, nextScriptToRunId));
            if (start) StartScript(id);
        }

        public void StartScript(string id)
        {
            ScriptStatus ss = _scripts.FirstOrDefault(s => s.Id == id);
            //TODO: use IndexOf so _currentScript can be assigned
            if (ss != null) ss.Script.Start();
        }

        public void StopScript(string id)
        {
            ScriptStatus ss = _scripts.FirstOrDefault(s => s.Id == id);
            if (ss != null) ss.Script.End();
        }

        private void InternalProcess()
        {
            while(_canRun)
            {
                //add a mechanism of starting inactive scripts when added with no autostart;
                //add a bool var?

                //AutoRun of the next script
                //for (int i = 0; i < _scripts.Count; i++)
                //{
                //    if (_scripts[i].Processed) return;

                //    if(_scripts[i].Script.HasFinished)
                //    {
                //        if (_scripts[i].NextScriptToRunId != "")
                //        {
                //            StartScript(_scripts[i].NextScriptToRunId);
                //        }

                //        _scripts[i].Processed = true;
                //    }
                //}

                GameFiber.Yield();
            }
        }
    }
}
