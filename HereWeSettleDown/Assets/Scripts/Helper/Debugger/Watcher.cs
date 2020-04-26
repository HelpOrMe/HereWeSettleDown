using System;
using System.Diagnostics;

namespace Helper.Debugging
{
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

        public static void WatchRun(Action action, string name = null)
        {
            Watcher watcher = new Watcher(action, name);
            Log.Info(watcher.StartLogText());
            watcher.Start();
            Log.Info(watcher.EndLogText());
        }

        public static void WatchRun(params Action[] actions)
        {
            foreach (Action action in actions)
            {
                Watcher watcher = new Watcher(action, null);
                Log.Info(watcher.StartLogText());
                watcher.Start();
                Log.Info(watcher.EndLogText());
            }
        }

        public Stopwatch Start()
        {
            stopwatch.Reset();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch;
        }

        public string StartLogText()
        {
            string mName = name ?? action.Method.Name;
            return $"{mName} started.";
        }

        public string EndLogText()
        {
            string mName = name ?? action.Method.Name;
            return $"{mName} completed after {stopwatch.ElapsedMilliseconds} ms.";
        }
    }
}

