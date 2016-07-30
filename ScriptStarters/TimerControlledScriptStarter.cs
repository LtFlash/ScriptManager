using Rage;

namespace ScriptManager.ScriptStarters
{
    internal class TimerControlledScriptStarter : ScriptStarterBase
    {
        //PRIVATE
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private double _intervalMin, _intervalMax;

        public TimerControlledScriptStarter(Managers.ScriptStatus ss, bool autoRestart = true) : base(ss, autoRestart)
        {
            _intervalMin = ss.TimerIntervalMin;
            _intervalMax = ss.TimerIntervalMax;

            _timer.Interval = GetRandomInterval(_intervalMin, _intervalMax);
            _timer.Elapsed += TimerTick;
            _timer.AutoReset = true;
        }

        private void TimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            Game.LogVerbose(nameof(TimerControlledScriptStarter) + "." + nameof(TimerTick));

            if(!ScriptStarted || script.HasFinishedUnsuccessfully)
            {
                StartScriptInThisTick = true;
                Game.LogVerbose(nameof(TimerControlledScriptStarter) + "." + nameof(TimerTick) + ":" + ScriptStarted);
            }
            else if(script.HasFinishedSuccessfully)
            {
                _timer.Stop();
            }

            _timer.Interval = GetRandomInterval(_intervalMin, _intervalMax);
        }

        public override void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private double GetRandomInterval(double min, double max)
        {
            return MathHelper.GetRandomDouble(min, max);
        }
    }
}
