using System;
using Rage;

namespace ScriptManager.Managers
{
    public class ScriptManager : ScriptManagerBase
    {
        private System.Timers.Timer _scriptRunTimer = new System.Timers.Timer();
        private double _intervalMax = 3 * 60 * 1000;
        private double _intervalMin = 1 * 60 * 1000;

        public ScriptManager(bool autoStart = false, double intervalMax = 180000, double intervalMin = 60000) : base()
        {
            _intervalMax = intervalMax;
            _intervalMin = intervalMin;

            if (autoStart) StartTimer();
        }

        public void StartTimer()
        {
            _scriptRunTimer.Interval = GetRandomTimerInterval();
            _scriptRunTimer.Elapsed += StartNextScript;
            _scriptRunTimer.AutoReset = true;
            _scriptRunTimer.Start();
        }

        private double GetRandomTimerInterval()
        {
            return MathHelper.GetRandomDouble(_intervalMin, _intervalMax);
        }

        private void StartNextScript(object sender, System.Timers.ElapsedEventArgs e)
        {
            Scripts.IBaseScript _scriptToRun = GetNextScriptReadyToRun();

            if(_scriptToRun != null && _scriptToRun.CanBeStarted())
            {
                _scriptToRun.Start();
            }

            _scriptRunTimer.Interval = GetRandomTimerInterval();
            Game.LogVerbose("ScriptManager.StartNextScript");
        }

        private Scripts.IBaseScript GetNextScriptReadyToRun()
        {//FindLastIndex returns -1 when no id found
            int _idLastScript = _scripts.FindLastIndex(s => s.Script.HasFinished);

            //no index found
            if (_idLastScript == -1) return _scripts.Count > 0 ? _scripts[0].Script : null;

            //last script on the list
            if (_scripts.Count - 1 == _idLastScript) return null;

            int _idNextScriptToRun = _scripts[_idLastScript].Script.Completed ? _idLastScript + 1 : _idLastScript;

            if (_scripts.Count - 1 >= _idNextScriptToRun) return _scripts[_idNextScriptToRun].Script;
            else return null;
        }
    }
}
