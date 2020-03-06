﻿using System;
using System.Diagnostics;

namespace Helper.Debugger
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

        public static void Watch(Action action, string name = null)
        {
            Watcher watcher = new Watcher(action, name);
            watcher.Start();
            watcher.Log();
        }

        public Stopwatch Start()
        {
            stopwatch.Reset();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch;
        }

        public string LogText()
        {
            string mName = name == null ? action.Method.Name : name;
            return $"{mName} ends after {stopwatch.ElapsedMilliseconds} mS.";
        }

        public void Log()
        {
            UnityEngine.Debug.Log(LogText());
        }
    }
}
