using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace ScriptManager.ScriptStarters
{
    internal class TimeControlledScriptStarter : ScriptStarterBase
    {
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private double _intervalMin, _intervalMax;
        private Managers.ScriptManagerBase.ScriptStatus _ss;

        public TimeControlledScriptStarter(Managers.ScriptManagerBase.ScriptStatus ss, double intervalMax, double intervalMin, bool autoRestart = true)
        {
            _ss = ss;
            _intervalMin = intervalMin;
            _intervalMax = intervalMax;

            _timer.Interval = GetRandomInterval(_intervalMin, _intervalMax);
            _timer.Elapsed += TimerTick;
        }

        private void TimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            //if(_ss.Start(true))

                //TODO: return bool if stared properly?
        }

        public void Start()
        {
            _timer.Start();
        }

        private double GetRandomInterval(double min, double max)
        {
            return MathHelper.GetRandomDouble(min, max);
        }
    }
}
