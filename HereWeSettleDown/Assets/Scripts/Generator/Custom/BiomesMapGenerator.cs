using System.Collections.Generic;
using UnityEngine;

namespace Generator.Custom
{
    [WorldGenerator(9, "mapWidth", "mapHeight")]
    public class BiomesMapGenerator : SubGenerator
    {
        public bool EvaluteMapHeightByBiomes;
        public Biome[] biomes;

        private Dictionary<int, Biome> indToBiome = new Dictionary<int, Biome>();

        private void Awake()
        {
            values["biomes"] = biomes;
        }

        public override void OnRegistrate()
        {
            if (EvaluteMapHeightByBiomes)
                HeightMapGenerator.RegistrateFunc(EvaluteHeightByBiomes, 1);
        }

        public override void OnGenerate()
        {
            int width = GetValue<int>("mapWidth");
            int height = GetValue<int>("mapHeight");

            GenerateBiomeMasks(width, height);
            GenerateGlobalMask(width, height);

            GenerationCompleted();
        }

        public void GenerateBiomeMasks(int width, int height)
        {
            BiomeMask[] biomeMasks = new BiomeMask[biomes.Length];
            for (int i = 0; i < biomes.Length; i++)
            {
                if (!indToBiome.ContainsKey(biomes[i].index))
                    indToBiome[biomes[i].index] = biomes[i];

                int[,] intMask = new int[width, height];
                BiomeMaskGeneration biomeSettings = biomes[i].maskSettings;

                float[,] heightMap = Noise.GenerateNoiseMap(width, height, biomeSettings.settings);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if ((heightMap[x, y] > biomeSettings.minHeightMask && 
                            heightMap[x, y] < biomeSettings.maxHeightMask) ||
                            biomes[i].absolute)
                        {
                            intMask[x, y] = biomes[i].index;
                        }
                    }
                }

                biomeMasks[i] = new BiomeMask(intMask);
            }
            values["biomeMasks"] = biomeMasks;
        }
        
        public void GenerateGlobalMask(int width, int height)
        {
            BiomeMask[] biomeMasks = GetValue<BiomeMask[]>("biomeMasks");

            int[,] globalMask = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < biomeMasks.Length; i++)
                    {
                        if (biomeMasks[i].mask[x, y] != 0)
                            globalMask[x, y] = biomeMasks[i].mask[x, y];
                    }
                }
            }

            values["globalMask"] = globalMask;
        }

        public float EvaluteHeightByBiomes(float[,] map, int x, int y)
        {
            int[,] globalMask = GetValue<int[,]>("globalMask");

            if (indToBiome.ContainsKey(globalMask[x, y]))
                return indToBiome[globalMask[x, y]].evaluteWorldHeight.Evaluate(map[x, y]);
            return map[x, y];
        }
    }

    [System.Serializable]
    public struct Biome
    {
        public string name;
        public Color mapColor;
        public int index;
        public bool absolute;

        public AnimationCurve evaluteWorldHeight;
        public BiomeMaskGeneration maskSettings;

        public BiomeColorRegion[] colorRegions;
    }

    [System.Serializable]
    public struct BiomeColorRegion
    {
        public string name;
        public float height;
        public Color color;
    }

    [System.Serializable]
    public struct BiomeMaskGeneration
    {
        [Range(0, 1)]
        public float minHeightMask;
        [Range(0, 1)]
        public float maxHeightMask;
        public GenerationSettings settings;
    }

    public struct BiomeMask
    {
        public int[,] mask;
        
        public BiomeMask(int[,] mask)
        {
            this.mask = mask;
        }
    }
}
