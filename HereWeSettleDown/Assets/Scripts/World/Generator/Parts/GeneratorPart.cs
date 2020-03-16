using System;
using Settings;

namespace World.Generator
{
    public abstract class GeneratorPart
    {
        protected readonly BaseGeneratorSettings settings = SettingsObject.GetObject<BaseGeneratorSettings>();

        public abstract void Run();
    }
}
