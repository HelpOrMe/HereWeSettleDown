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

        public BiomeGenerationSettings settings;
    }

    [System.Serializable]
    public struct BiomeGenerationSettings
    {
        public float noiseScale;
        public AnimationCurve heightCurve;
        public int octaves;
        [Range(0f, 1f)]
        public float persistance;
        public float lacunarity;
        public Vector2 offset;
    }
}
