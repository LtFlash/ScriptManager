using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptManager.ScriptStarters
{
    internal abstract class ScriptStarterBase : IScriptStarter
    {
        //PUBLIC
        public bool FinishedSuccessfully { get { return script.HasFinishedSuccessfully; } }
        public bool FinishedUnsuccessfully
        {
            get { return _finishedUnsuccessfully || script.HasFinishedUnsuccessfully; }
            protected set { _finishedUnsuccessfully = value; }
        }

        public string Id { get { return script.Id; } }
        public string[] NextScriptsToRun { get { return script.NextScriptToRunIds; } }

        //PROTECTED
        protected Managers.ScriptStatus script;
        protected bool StartScriptInThisTick { get; set; }
        protected bool ScriptStarted { get; private set; }
        protected bool AutoRestart { get; private set; }
        protected Resources.ProcessHost Stages { get; private set; } = new Resources.ProcessHost();

        //PRIVATE
        private bool _finishedUnsuccessfully;

        public ScriptStarterBase(Managers.ScriptStatus s, bool autoRestart)
        {
            script = s;

            AutoRestart = autoRestart;

            Stages.AddProcess(InternalProcess);
            Stages.ActivateProcess(InternalProcess);
            Stages.Start();
        }

        public abstract void Start();

        private void InternalProcess()
        {
            if(StartScriptInThisTick/* && ss.IsRunning*/)
            {
                ScriptStarted = script.Start();
                StartScriptInThisTick = false;
            }
        }
    }
}
