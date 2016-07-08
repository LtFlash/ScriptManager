namespace ScriptManager.Example
{
    class Homicide : Scripts.CalloutScriptBase
    {
        Managers.ScriptManager _stagesOfInvestigation = new Managers.ScriptManager();

        public Homicide()
        {
            _stagesOfInvestigation.AddScript("csi", new Homicide_CSI(), true);
        }

        public override void Process()
        {
            base.Process();
        }

        public override void End()
        {
            base.End();
        }
    }
}
