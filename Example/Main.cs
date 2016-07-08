namespace ScriptManager.Example
{
    class Main
    {
        Managers.ScriptManager _cases = new Managers.ScriptManager();

        public void AddCases()
        {
            _cases.AddScript("homicide", new Homicide(), true);
        }
    }
}
