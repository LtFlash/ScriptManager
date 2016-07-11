using System;

namespace ScriptManager.Scripts
{
    /// <summary>
    /// Common interface for different types of scripts.
    /// </summary>
    public interface IBaseScript 
    {
        bool IsRunning { get; }
        bool HasFinished { get; }
        bool Completed { get; }
        bool CanBeStarted();
        void Start();
        //void Initialize();
        //void Process();
        //void End();
        void SetScriptFinished(bool completed);
    }
    /// <summary>
    /// Callout-like script.
    /// </summary>
    public interface ICalloutScript : IBaseScript
    {
        //void Accepted();
        //void NotAccepted();
    }

    public interface ICalloutHourRangeScriptBase : ICalloutScript
    {
        TimeSpan HourStart { get; }
        TimeSpan HourEnd { get; }
    }
}
