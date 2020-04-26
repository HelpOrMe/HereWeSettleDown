using System;
using System.Collections.Generic;
using System.Threading;
using Helper.Threading;
using UnityEngine;

namespace Helper.Debugging
{
    public static class Log
    {
        // Logger class, works in sub threads

        public static string defaultFormat = "[{0}] {1}";
        public static string setFromat = defaultFormat + " has been set.";


        // Info shortcuts
        public static void Info(object[] objects, string sep, string format, string threadName) => LogMethod(Debug.Log, objects, sep, format, threadName);
        public static void Info(params object[] objects) => Info(objects, " ", defaultFormat, Thread.CurrentThread.Name);
        public static void InfoSet(params object[] objects) => Info(objects, " ", setFromat, Thread.CurrentThread.Name);

        // Warning shortcuts
        public static void Warning(object[] objects, string sep, string format, string threadName) => LogMethod(Debug.LogWarning, objects, sep, format, threadName);
        public static void Warning(params object[] objects) => Warning(objects, " ", defaultFormat, Thread.CurrentThread.Name);

        // Error shortcuts
        public static void Error(object[] objects, string sep, string format, string threadName) => LogMethod(Debug.LogError, objects, sep, format, threadName);
        public static void Error(params object[] objects) => Error(objects, " ", defaultFormat, Thread.CurrentThread.Name);

        // Send log message with logMethod (Debug.Log, Debug.LogWarning, etc.) 
        public static void LogMethod(Action<string> logMethod, object[] objects, string sep, string format, string threadName)
        {
            if (!MainThreadInvoker.CheckForMainThread())
            {
                MainThreadInvoker.InvokeAction(() => LogMethod(logMethod, objects, sep, format, threadName));
                return;
            }

            if (string.IsNullOrEmpty(threadName))
                threadName = "Main thread";

            string text = GetText(objects, sep);
            string formattedText = Formatter(text, format, threadName);
            logMethod(formattedText);
        }

        // Convert objects array to string
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

        private static string Formatter(string text, string format, string threadName)
        {
            // Simple formatter
            return string.Format(format, threadName, text);
        }
    }
}
