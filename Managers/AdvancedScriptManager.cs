﻿using System;
using System.Collections.Generic;
using System.Linq;
using Rage;
using ScriptManager.ScriptStarters;

namespace ScriptManager.Managers
{
    public class AdvancedScriptManager
    {
        //PUBLIC
        public double DefaultTimerIntervalMax { get; set; } = 30000;
        public double DefaultTimerIntervalMin { get; set; } = 15000;
        public bool IsRunning { get; private set; }
        public bool AutoSwapFromSequentialToTimer { get; set; } = true;

        //PRIVATE
        private List<ScriptStatus> _off = new List<ScriptStatus>();
        private List<ScriptStatus> _queue = new List<ScriptStatus>();
        private List<IScriptStarter> _running = new List<IScriptStarter>();

        private Dictionary<string, bool> _statusOfScripts = new Dictionary<string, bool>();

        private Resources.ProcessHost _stages = new Resources.ProcessHost();

        public AdvancedScriptManager()
        {
            _stages.AddProcess(Process_RunScriptsFromQueue);
            _stages.AddProcess(Process_WaitScriptsForFinish);
        }

        public void AddScript(Type typeImplIScript, string id, Scripts.EInitModels initModel, 
            string[] nextScriptsToRun, List<string[]> scriptsToFinishPrior, double timerIntervalMin, double timerIntervalMax)
        {
            if (!typeImplIScript.GetInterfaces().Contains(typeof(Scripts.IScript)))
            {
                throw new ArgumentException(
                    $"Parameter does not implement {nameof(Scripts.IScript)} interface.",
                    nameof(typeImplIScript));
            }

            ScriptStatus s = new ScriptStatus(
                id, typeImplIScript, initModel,
                nextScriptsToRun, scriptsToFinishPrior,
                timerIntervalMin, timerIntervalMax);

            AddNewScriptToList(s, id);
        }

        public void AddScript(Type typeBaseScript, string id, Scripts.EInitModels initModel)
        {
            AddScript(
                typeBaseScript, id, initModel,
                new string[0], new List<string[]>(),
                DefaultTimerIntervalMin, DefaultTimerIntervalMin);
        }

        public void Start() 
        {
            StartScript(_off.First().Id);
        }

        public void StartScript(string id)
        {
            //clear prior list to prevent blockage
            GetScriptById(id, _off).ScriptsToFinishPriorThis = new List<string[]>();

            MoveInactiveScriptToQueue(id, _off, _queue);

            RegisterProcesses();
            IsRunning = true;
        }

        public bool HasScriptFinished(string id)
        {
            if(!_statusOfScripts.ContainsKey(id))
            {
                throw new ArgumentException($"{nameof(HasScriptFinished)}: Script with id [{id}] does not exist.");
            }

            return _statusOfScripts[id];
        }

        private void RegisterProcesses()
        {
            _stages.ActivateProcess(Process_RunScriptsFromQueue);
            _stages.ActivateProcess(Process_WaitScriptsForFinish);
            _stages.Start();
        }

        private void Process_RunScriptsFromQueue()
        {
            for (int i = 0; i < _queue.Count; i++)
            {
                if(CheckIfScriptCanBeStarted(_queue[i]))
                {
                    MoveScriptFromQueueToRunning(_queue[i], _queue, _running);
                }
            }
        }

        private void Process_WaitScriptsForFinish()
        {
            List<IScriptStarter> fs = GetSuccessfullyFinishedScripts(_running);

            if (fs.Count < 1) return;

            SetScriptStatusAsFinished(fs);

            for (int i = 0; i < fs.Count; i++)
            {
                AddScriptsToQueue(fs[i].NextScriptsToRun);
            }

            RemoveScripts(fs, _running);
        }

        private void AddNewScriptToList(ScriptStatus script, string id)
        {
            _off.Add(script);
            _statusOfScripts.Add(id, false);
        }

        private bool CheckIfScriptCanBeStarted(ScriptStatus script)
        {
            if (script.ScriptsToFinishPriorThis.Count < 1) return true;
            else return CheckIfNecessaryScriptsAreFinished(script.ScriptsToFinishPriorThis, _statusOfScripts);
        }

        private ScriptStatus GetScriptById(string id, List<ScriptStatus> from)
        {
            if(from.FirstOrDefault(s => s.Id == id) == null)
            {
                throw new ArgumentException($"{nameof(GetScriptById)}: Script with id [{id}] does not exist.");
            }

            return from.Find(s => s.Id == id);
        }

        private IScriptStarter CreateScriptStarterFromFirst(List<ScriptStatus> scriptsToRun)
        {
            IScriptStarter result = null;

            if (scriptsToRun.Count < 1) return result;

            result = CreateScriptStarter(scriptsToRun.First());

            return result;
        }

        private IScriptStarter CreateScriptStarterByScriptId(string id, List<ScriptStatus> scriptsToRun)
        {
            //GetScriptById does not return null, it throws an exception instead
            ScriptStatus s = GetScriptById(id, scriptsToRun); 
            return CreateScriptStarter(s);
        }

        private List<IScriptStarter> CreateScriptsStartersByIds(string[] ids, List<ScriptStatus> scripts)
        {
            List<IScriptStarter> result = new List<IScriptStarter>();

            for (int i = 0; i < ids.Length; i++)
            {
                IScriptStarter ss = CreateScriptStarterByScriptId(ids[i], scripts);

                result.Add(ss);
            }

            return result;
        }

        private IScriptStarter CreateScriptStarter(ScriptStatus ss)
        {
            switch (ss.InitModel)
            {
                case Scripts.EInitModels.Sequential:
                    return null;

                case Scripts.EInitModels.TimerBased:
                default:
                    return new TimerControlledScriptStarter(ss, true);
            }
        }

        private bool CheckIfNecessaryScriptsAreFinished(List<string[]> scripts, Dictionary<string, bool> status)
        {
            //algorithm tested, works
            List<bool> arrays = new List<bool>();

            for (int i = 0; i < scripts.Count; i++)
            {
                //TODO: protection check for non-existant key - no sense in running
                //the mod when a crucial script might be missing?
                //implement a function to CheckIfAll()?
                arrays.Add(scripts[i].All(s => status[s])); 
            }

            return arrays.Any(b => b == true);
        }

        private void StartScripts(List<IScriptStarter> starters)
        {
            for (int i = 0; i < starters.Count; i++)
            {
                starters[i].Start();
            }
        }

        private void RemoveSuccessfullyFinishedScripts(List<IScriptStarter> running)
        {
            //NOTE: replace with RemoveScripts wherever possible to avoid unnecessary calculations
            for (int i = 0; i < running.Count; i++)
            {
                if (running[i].FinishedSuccessfully)
                {
                    running.RemoveAt(i);
                }
            }
        }

        private void RemoveScripts(List<IScriptStarter> scriptsToRemove, List<IScriptStarter> from)
        {
            for (int i = 0; i < scriptsToRemove.Count; i++)
            {
                from.Remove(scriptsToRemove[i]);
            }
        }

        private List<IScriptStarter> GetSuccessfullyFinishedScripts(List<IScriptStarter> running)
        {
            List<IScriptStarter> result = new List<IScriptStarter>();

            for (int i = 0; i < running.Count; i++)
            {
                if(running[i].FinishedSuccessfully)
                {
                    result.Add(running[i]);

                    Game.LogVerbose(nameof(AdvancedScriptManager) + "." + nameof(GetSuccessfullyFinishedScripts) + ":" + running[i].Id);
                }
            }

            return result;	
        }

        private void SetScriptStatusAsFinished(List<IScriptStarter> scripts)
        {
            for (int i = 0; i < scripts.Count; i++)
            {
                _statusOfScripts[scripts[i].Id] = true;
            }
        }

        private void MoveInactiveScriptToQueue(string scriptId, List<ScriptStatus> from, List<ScriptStatus> to)
        {
            ScriptStatus s = GetScriptById(scriptId, from);
            to.Add(s);
            from.Remove(s);
            Game.LogVerbose(nameof(AdvancedScriptManager) + "." + nameof(MoveInactiveScriptToQueue) + ":" + s.Id);
        }

        private void MoveScriptFromQueueToRunning(ScriptStatus scriptToRun, List<ScriptStatus> from, List<IScriptStarter> to)
        {
            IScriptStarter s = CreateScriptStarter(scriptToRun);
            s.Start();
            to.Add(s);
            from.Remove(scriptToRun);
            Game.LogVerbose(nameof(AdvancedScriptManager) + "." + nameof(MoveScriptFromQueueToRunning) + ":" + s.Id);
        }

        private void AddScriptsToQueue(string[] scriptsToRun)
        {
            for (int i = 0; i < scriptsToRun.Length; i++)
            {
                MoveInactiveScriptToQueue(scriptsToRun[i], _off, _queue);
            }
        }
    }
}
