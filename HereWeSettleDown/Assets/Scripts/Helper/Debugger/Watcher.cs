using System;
using System.Diagnostics;

namespace Helper.Debugging
{
    /// <summary>
    /// Class helper to run action/s with logging.
    /// </summary>
    public class Watcher
    {
        public readonly Action action;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private string name;

        public Watcher(Action action, string name = null)
        {
            this.action = action;
            this.name = name;
        }

        /// <summary>
        /// Run action with logging.
        /// The same as Watcher().Start()
        /// </summary>
        /// <param name="action">Target action</param>
        /// <param name="name">Logging name</param>
        public static void WatchRun(Action action, string name = null)
        {
            Watcher watcher = new Watcher(action, name);
            Log.SetWorker(name ?? action.Method.Name);
            Log.Info(watcher.StartLogText());
            watcher.Start();
            Log.Info(watcher.EndLogText());
        }

        /// <summary>
        /// Run several actions with logging.
        /// </summary>
        /// <param name="actions">Target actions</param>
        public static void WatchRun(params Action[] actions)
        {
            foreach (Action action in actions)
            {
                Log.SetWorker(action.Method.Name);
                Watcher watcher = new Watcher(action, null);
                Log.Info(watcher.StartLogText());
                watcher.Start();
                Log.Info(watcher.EndLogText());
            }
        }

        /// <summary>
        /// Run watcher action with logging.
        /// </summary>
        /// <returns>Stopwatch</returns>
        public Stopwatch Start()
        {
            stopwatch.Reset();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch;
        }

        private string StartLogText()
        {
            string mName = name ?? action.Method.Name;
            return $"{mName} started.";
        }

        private string EndLogText()
        {
            string mName = name ?? action.Method.Name;
            return $"{mName} completed after {stopwatch.ElapsedMilliseconds} ms.";
        }
    }
}

