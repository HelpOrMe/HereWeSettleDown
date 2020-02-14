using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class BiomesMapGenerator : MonoBehaviour
    {
        public Biome[] biomes;

        public BiomeMask[] GenerateBiomesMask(int width, int height)
        {
            BiomeMask[] masks = new BiomeMask[biomes.Length];
            
            for (int i = 0; i < biomes.Length; i++)
            {
                int[,] intMask = new int[width, height];

                BiomeMaskGeneration biomeSettings = biomes[i].maskSettings;
                GenerationSettings genSettings = biomeSettings.settings;

                float[,] heightMap = Noise.GenerateNoiseMap(
                    width, height, 
                    genSettings.noiseScale, genSettings.octaves, genSettings.persistance, 
                    genSettings.lacunarity, genSettings.offset);

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
                masks[i] = new BiomeMask(intMask);
            }

            return masks;
        }
        
        public float[,] EvaluteHeightByBiomes(float[,] map, int[,] globalMask)
        {
            // Generate biomes layers
            Dictionary<int, Biome> indToBiome = new Dictionary<int, Biome>();
            for (int i = 0; i < biomes.Length; i ++)
            {
                if (!indToBiome.ContainsKey(biomes[i].index))
                {
                    indToBiome[biomes[i].index] = biomes[i];
                }
            }

            // Set layers in map
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (indToBiome.ContainsKey(globalMask[x,y]))
                    {
                        map[x, y] = Mathf.Clamp01(indToBiome[globalMask[x, y]].evaluteWorldHeight.Evaluate(map[x, y]));
                    }
                }
            }

            return map;
        }

        public Color[] CreateColorMap(int[,] mask)
        {
            int width = mask.GetLength(0);
            int height = mask.GetLength(1);

            // Set dicitionary with index to colors
            Dictionary<int, Color> indToColor = new Dictionary<int, Color>();
            for (int i = 0; i < biomes.Length; i++)
                if (!indToColor.ContainsKey(biomes[i].index))
                    indToColor.Add(biomes[i].index, biomes[i].mapColor);

            // Create color map
            Color[] colorMap = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (indToColor.ContainsKey(mask[x, y]))
                        colorMap[y * width + x] = indToColor[mask[x, y]];
                }
            }

            return colorMap;
        }

        public int[,] СombineMasks(int width, int height, BiomeMask[] biomeMasks)
        {
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

            return globalMask;
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
