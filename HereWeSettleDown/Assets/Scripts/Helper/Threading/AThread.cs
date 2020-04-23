using Helper.Scene;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Helper.Threading
{
    [Serializable]
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
            thread = new Thread(start)
            {
                Name = start.GetType().Name
            };
        }

        public AThread(params Action[] actions)
        {
            this.actions = actions;
            actionsStart = true;

            thread = new Thread(ActionsInvoker)
            {
                Name = "Actions Thread"
            };
        }

        private void ActionsInvoker()
        {
            foreach (Action action in actions)
            {
                action();
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

