using System;
using System.Collections.Generic;
using System.Threading;
using Helper.Threading;
using UnityEngine;

namespace Helper.Debugging
{
    /// <summary>
    /// Logger class, works in sub threads
    /// </summary>
    public static class Log
    {
        public static string defaultFormat = "[{0}] [{1}] {2}";
        public static string setFromat = defaultFormat + " has been set.";
        private static string worker = "Main";

        // Info shortcuts
        /// <summary>
        /// LogMethod info shortcut
        /// </summary>
        public static void Info(object[] objects, string sep, string format, string threadName) => LogMethod(Debug.Log, objects, sep, format, worker, threadName);
        /// <summary>
        /// LogMethod info shortcut
        /// Uses Log.defaultFormat
        /// </summary>
        public static void Info(params object[] objects) => Info(objects, " ", defaultFormat, Thread.CurrentThread.Name);
        /// <summary>
        /// LogMethod set info shortcut
        /// Uses Log.setFormat
        /// </summary>
        public static void InfoSet(params object[] objects) => Info(objects, " ", setFromat, Thread.CurrentThread.Name);

        // Warning shortcuts
        /// <summary>
        /// LogMethod warn shortcut
        /// </summary>
        public static void Warning(object[] objects, string sep, string format, string threadName) => LogMethod(Debug.LogWarning, objects, sep, format, worker, threadName);
        /// <summary>
        /// LogMethod warn shortcut
        /// Uses Log.defaultFormat
        /// </summary>
        public static void Warning(params object[] objects) => Warning(objects, " ", defaultFormat, Thread.CurrentThread.Name);

        // Error shortcuts
        /// <summary>
        /// LogMethod error shortcut
        /// </summary>
        public static void Error(object[] objects, string sep, string format, string threadName) => LogMethod(Debug.LogError, objects, sep, format, worker, threadName);
        /// <summary>
        /// LogMethod error shortcut
        /// Uses Log.defaultFormat
        /// </summary>
        public static void Error(params object[] objects) => Error(objects, " ", defaultFormat, Thread.CurrentThread.Name);

        /// <summary>
        /// Send log message with logMethod
        /// </summary>
        /// <param name="logMethod">Debug.Log, Debug.LogWarning, etc.</param>
        /// <param name="objects">ToString objects (+arrays)</param>
        /// <param name="sep">Separator</param>
        /// <param name="format">Format string</param>
        /// <param name="threadName">Current thread name</param>
        public static void LogMethod(Action<string> logMethod, object[] objects, string sep, string format, string worker, string threadName)
        {
            if (!MainThreadInvoker.CheckForMainThread())
            {
                MainThreadInvoker.InvokeAction(() => LogMethod(logMethod, objects, sep, format, worker, threadName));
                return;
            }

            if (string.IsNullOrEmpty(threadName))
                threadName = "Main thread";

            string text = GetText(objects, sep);
            string formattedText = Formatter(text, format, worker, threadName);
            logMethod(formattedText);
        }

        /// <summary>
        /// Set logger worker
        /// </summary>
        public static void SetWorker(string name) => worker = name;

        /// <summary>
        /// Reset current worker
        /// </summary>
        public static void ResetWorker() => worker = "Main";

        /// <summary>
        /// Convert objects array to string
        /// </summary>
        private static string GetText(IEnumerable<object> objects, string sep)
        {
            string res = "";
            foreach (object item in objects)
            {
                string text;
                if (item.GetType().IsArray)
                    text = GetText((IEnumerable<object>)item, sep);
                else
                    text = item.ToString();
                res += text + sep;
            }
            
            return res.Remove(res.Length - 1);
        }

        /// <summary>
        /// Simple formater
        /// </summary>
        private static string Formatter(string text, string format, string worker, string threadName)
        {
            return string.Format(format, threadName, worker, text);
        }
    }
}
