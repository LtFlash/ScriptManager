namespace ScriptManager.Example
{
    class Main
    {
        Managers.TimerBasedScriptManager _cases = new Managers.TimerBasedScriptManager();

        public void AddCases()
        {
            _cases.AddScript("homicide", typeof(Homicide));
            _cases.StartScript("homicide", true);
        }
    }
}
