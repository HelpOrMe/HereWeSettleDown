using UnityEngine;

namespace Generator
{
    [System.Serializable]
    public struct Biome
    {
        public string name;
        public Color mapColor;
        public int index;
        public int countOfBiomes;

        public AnimationCurve heightCurve;
    }
}
