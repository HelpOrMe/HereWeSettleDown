using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using UnityEngine;
using Helper.Scene;
using Helper.Debugger;

namespace Helper.Threading
{
    public class AThread
    {
        public string Name
        {
            get => thread.Name;
            set => thread.Name = value;
        }
        public readonly Thread thread;

        private Action[] actions;
        private bool actionsStart = false;

        private Stopwatch debugStopwatch = new Stopwatch();

        public AThread(ParameterizedThreadStart start)
        {
            thread = new Thread(start);
            thread.Name = start.GetType().Name;
        }

        public AThread(params Action[] actions)
        {
            this.actions = actions;
            actionsStart = true;

            thread = new Thread(ActionsInvoker);
            thread.Name = "Actions Thread";
        }
        
        private void ActionsInvoker()
        {
            Stopwatch stopwatch = new Stopwatch();
            foreach (Action action in actions)
            {
                Watcher watcher = new Watcher(action);
                watcher.Start();
                UnityEngine.Debug.Log($"{Name} > {watcher.LogText()}");
            }
        }

        public void Start(object parameter)
        {
            if (actionsStart)
                thread.Start();
            else
                thread.Start(parameter);
            StartDebug();
        }

        public void Start()
        {
            thread.Start();
            StartDebug();
        }

        public void RunAfterThreadEnd(Action action)
        {
            ObjectFinder.FindRootSceneObject<AThreadMono>().StartCoroutine(RunAfterThreadEndRoutine(action));
        }

        private IEnumerator RunAfterThreadEndRoutine(Action action)
        {
            yield return new WaitUntil(() => thread.IsAlive);
            yield return new WaitUntil(() => !thread.IsAlive);
            action();
        }

        private void StartDebug()
        {
            UnityEngine.Debug.Log(Name + " starts");
            debugStopwatch.Start();
            RunAfterThreadEnd(EndDebug);
        }

        private void EndDebug()
        {
            debugStopwatch.Stop();
            UnityEngine.Debug.Log($"The {Name} ended after {debugStopwatch.ElapsedMilliseconds} mS.");
        }
    }
}

