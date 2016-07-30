using System;

namespace ScriptManager.Example
{
    class Homicide : Scripts.ScriptBase
    {
        Managers.TimerBasedScriptManager _stagesOfInvestigation = new Managers.TimerBasedScriptManager();

        public Homicide()
        {
            _stagesOfInvestigation.AddScript("csi", typeof(Homicide_CSI));
            _stagesOfInvestigation.StartScript("csi", false);
        }

        protected override bool Initialize()
        {
            return true;
        }

        protected override void Process()
        {
        }

        protected override void End()
        {
        }
    }
}
