using Helper.Threading;
using Settings;
using Settings.Generator;
using System;

namespace World.Generator
{
    [Serializable]
    public class GeneratorPart
    {
        public Action action => GetAction();
        public bool RunInNewThread = false;
        protected readonly BaseGeneratorSettings settings = SettingsObject.GetObject<BaseGeneratorSettings>();

        protected virtual void Run() { }

        public Action GetAction()
        {
            if (RunInNewThread)
            {
                return new AThread(Run).Start;
            }

            return Run;
        }
    }
}
