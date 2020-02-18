using UnityEngine;

namespace Generator.Custom
{
    public class Biome : MonoBehaviour
    {
        public string name;
        public Color mapColor;
        public int index;

        public bool absolute;

        public bool spawnOnAllBiomes;
        public Biome[] spawnOnBiomes;

        public bool anySpawnHeight;
        [Range(0, 1)] public float minSpawnHeight;
        [Range(0, 1)] public float maxSpawnHeight;

        [Range(0, 1)] public float minMaskHeight;
        [Range(0, 1)] public float maxMaskHeight;
        public bool copyBiomeMask;
        public Biome biomeMaskParent;
        public GenerationSettings maskSettings;

        public bool isEvaluteHeight;
        public AnimationCurve evaluteHeight;

        public bool overrideColors;
        public BiomeColorRegion[] colorRegions;

        public GenerationSettings GetBiomeMask()
        {
            if (copyBiomeMask)
                return biomeMaskParent.GetBiomeMask();
            else
                return maskSettings;
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