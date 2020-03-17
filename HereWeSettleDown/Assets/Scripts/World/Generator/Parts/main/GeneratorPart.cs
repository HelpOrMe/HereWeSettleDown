using System;
using Settings;
using Settings.Generator;
using Helper.Threading;

namespace World.Generator
{
    [Serializable]
    public class GeneratorPart
    {
        public Action action { get => GetAction(); }
        public bool RunInNewThread = false;
        protected readonly BaseGeneratorSettings settings = SettingsObject.GetObject<BaseGeneratorSettings>();

        protected virtual void Run() { }

        public Action GetAction()
        {
            if (RunInNewThread)
                return new AThread(Run).Start;
            return Run;
        }
    }
}
