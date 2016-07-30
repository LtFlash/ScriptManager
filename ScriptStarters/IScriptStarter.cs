
namespace ScriptManager.ScriptStarters
{
    interface IScriptStarter
    {
        bool FinishedSuccessfully { get; }
        bool FinishedUnsuccessfully { get; }
        string Id { get; }
        string[] NextScriptsToRun { get; }
        void Start();
    }
}
