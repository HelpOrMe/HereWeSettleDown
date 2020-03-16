using UnityEngine;
using Settings;

namespace World.Generator
{
    [CreateAssetMenu(menuName = "Settings/BaseGeneratorSettings")]
    public class BaseGeneratorSettings : SettingsObject
    {
        public int cellsCount = 4096;

        public int worldWidth = 512;
        public int worldHeight = 512;

        public int chunkWidth = 64;
        public int chunkHeight = 64;

        public int heightOffset = 3;
        public int heightSmoothIterations = 2;
        public float heightSmoothCoef = 0.4f;
    }
}
