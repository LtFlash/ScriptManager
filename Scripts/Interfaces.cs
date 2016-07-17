using System;

namespace ScriptManager.Scripts
{
    /// <summary>
    /// Common interface for different types of scripts.
    /// </summary>
    public interface IScript 
    {
        bool IsRunning { get; }
        bool HasFinished { get; }
        bool Completed { get; }
        bool CanBeStarted();
        void Start();
        void SetScriptFinished(bool completed);
    }
    /// <summary>
    /// Callout-like script.
    /// </summary>
    public interface ICalloutScript : IScript
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
