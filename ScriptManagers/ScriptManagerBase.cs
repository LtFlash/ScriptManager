using System.Collections.Generic;
using System.Linq;
using Rage;

namespace ScriptManager
{
    internal abstract class ScriptManagerBase
    {
        //PRIVATE
        private GameFiber _process;
        private List<ScriptStatus> _scripts = new List<ScriptStatus>();
        private int _currentScript = 0;
        private class ScriptStatus
        {
            public string Id;
            public Scripts.IBaseScript Script;
            public bool Status;
            //public string NextScriptToRunId;
            public ScriptStatus(string id, Scripts.IBaseScript script, bool isRunning)
            {
                Id = id;
                Script = script;
                Status = isRunning;
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

        public void AddScript(string id, Scripts.IBaseScript script, bool start = false)
        {
            _scripts.Add(new ScriptStatus(id, script, start));
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
                //next script will be automatically started -> make config for this
                //if(_scripts[_currentScript].Script.IsRunning)
                //{
                //    if(_scripts[_currentScript].Script.HasFinished)
                //    {
                //        if (_scripts.Count > _currentScript - 1)
                //        {
                //            _scripts[_currentScript].Script.Start();
                //        }
                //    }
                //}
                GameFiber.Yield();
            }
        }
    }
}
