using UnityEngine;

namespace Settings.Generator
{
    [CreateAssetMenu(menuName = "Settings/Generator/BaseGeneratorSettings")]
    public class BaseGeneratorSettings : SettingsObject
    {
        public int cellsCount = 4096;

        public int worldWidth = 512;
        public int worldHeight = 512;

        public int chunkWidth = 64;
        public int chunkHeight = 64;
    }
}
