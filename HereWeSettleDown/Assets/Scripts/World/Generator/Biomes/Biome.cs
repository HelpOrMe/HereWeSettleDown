using UnityEngine;
using World.Generator.HeightMap;
using World.Generator.Helper;

namespace World.Generator.Biomes
{
    public class Biome : MonoBehaviour
    {
        public Color mapColor;
        public int index;

        public bool SpawnOnAllBiomes;
        public Biome[] spawnOnBiomes;

        public bool UseDefaultHeightMap;
        public NoiseSettings noiseSettings;
        public AnimationCurve worldHeightCurve;
        public HeightMaskPattern heightMaskPattern;
        public float power;

        public bool OverrideColors;
        public BiomeColorRegion[] colorRegions;
    }

    [System.Serializable]
    public struct BiomeColorRegion
    {
        public string name;
        public float height;
        public Color color;
    }
}