﻿using Helper.Threading;
using Settings;
using Settings.Generator;
using System;

namespace World.Generator
{
    [Serializable]
    public class GeneratorPart
    {
        public bool RunInNewThread = false;

        protected BaseGeneratorSettings settings
        {
            get
            {
                if (_settings == null)
                    _settings = GameSettingsProvider.GetSettings<BaseGeneratorSettings>();
                return _settings;
            }
        }
        [NonSerialized] private BaseGeneratorSettings _settings;

        protected virtual void Run() { }

        public void Invoke()
        {
            if (RunInNewThread)
                new AThread(Run).Start();
            else Run();
        }

        public static void InvokePart<T>() where T : GeneratorPart
        {
            GeneratorPart part = (GeneratorPart)Activator.CreateInstance(typeof(T));
            part.Invoke();
        }

        public static void InvokePart(Type t)
        {
            if (t.IsSubclassOf(typeof(GeneratorPart)))
            {
                GeneratorPart part = (GeneratorPart)Activator.CreateInstance(t);
                part.Invoke();
            }
        }
    }
}
