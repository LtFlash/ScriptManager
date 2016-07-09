namespace ScriptManager.Managers
{
    public class SequentialScriptManager : ScriptManagerBase
    {
        public SequentialScriptManager() : base()
        {

        }

        protected override void Process()
        {
            for (int i = 0; i < _scripts.Count; i++)
            {
                if (_scripts[i].Processed) continue;

                if(_scripts[i].FinishedSuccessfully)
                {
                    StartScript(_scripts[i].NextScriptToRunId, true);

                    _scripts[i].Processed = true;
                }
                else if(_scripts[i].FinishedUnsuccessfully)
                {
                    //restart the same script
                    _scripts[i].Start(true);
                }
            }
        }
    }
}
