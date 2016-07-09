namespace ScriptManager.Example
{
    class Homicide : Scripts.CalloutScriptBase
    {
        Managers.TimerBasedScriptManager _stagesOfInvestigation = new Managers.TimerBasedScriptManager();

        public Homicide()
        {
            _stagesOfInvestigation.AddScript("csi", typeof(Homicide_CSI));
            _stagesOfInvestigation.StartScript("csi", false);
        }

        public override void Process()
        {
        }

        public override void End()
        {
        }
    }
}
