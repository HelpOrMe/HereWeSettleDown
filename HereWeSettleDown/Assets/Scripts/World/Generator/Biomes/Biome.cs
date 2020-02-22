using UnityEngine;

namespace World.Generator.Biomes
{
    public abstract class Biome : MonoBehaviour
    {
        public int index;
        public Color mapColor;
        public float power;
        
        public bool OverrideColors;
        public BiomeColorRegion[] colorRegions;

        public abstract bool GetNewHeightMap(System.Random prng, Biome[] biomes, ref float[,] heightMap, ref int[,] globalBiomeMask);

        public abstract float[,] GetBiomeMask();
    }

    [System.Serializable]
    public struct BiomeColorRegion
    {
        public string name;
        public float height;
        public Color color;
    }
}

