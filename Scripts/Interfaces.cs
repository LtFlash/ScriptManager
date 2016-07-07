namespace ScriptManager.Scripts
{
    /// <summary>
    /// Common interface for different types of scripts.
    /// </summary>
    internal interface IBaseScript 
    {
        bool IsRunning { get; }
        bool HasFinished { get; }
        void Start();
        void Initialize();
        void Process();
        void End();

    }
    /// <summary>
    /// Callout-like script.
    /// </summary>
    internal interface ICalloutScript : IBaseScript
    {
        void Accepted();
        void NotAccepted();
    }
}
