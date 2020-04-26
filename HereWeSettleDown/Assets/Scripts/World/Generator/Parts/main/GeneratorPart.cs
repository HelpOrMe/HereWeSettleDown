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

        protected BaseGeneratorSettings settings
        {
            get
            {
                if (_settings == null)
                    _settings = SerializedSettings.GetSettings<BaseGeneratorSettings>();
                return _settings;
            }
        }
        [NonSerialized] private BaseGeneratorSettings _settings;

        protected virtual void Run() { }

        public Action GetAction()
        {
            if (RunInNewThread)
                return new AThread(Run).Start;
            return Run;
        }
    }
}
