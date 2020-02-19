using UnityEngine;

namespace Generator.Custom
{
    public class Biome : MonoBehaviour
    {
        public string name;
        public Color mapColor;
        public int index;

        public bool spawnOnAllBiomes;
        public Biome[] spawnOnBiomes;

        public bool anySpawnHeight;
        [Range(-1, 1)] public float minSpawnHeight;
        [Range(-1, 1)] public float maxSpawnHeight;

        public HeightMaskPattern heightMaskPattern;

        public bool isEvaluteHeight;
        public AnimationCurve evaluteHeight;

        public bool overrideColors;
        public BiomeColorRegion[] colorRegions;

        public int[,] GetBiomeMask(System.Random prng, float[,] heightMap)
        {
            return heightMaskPattern.GetMask(prng, heightMap);
        }
    }

    [System.Serializable]
    public struct BiomeColorRegion
    {
        public string name;
        public float height;
        public Color color;
    }
}