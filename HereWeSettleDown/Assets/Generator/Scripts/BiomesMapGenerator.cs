using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class BiomesMapGenerator : MonoBehaviour
    {
        [HideInInspector] public int mapWidth;
        [HideInInspector] public int mapHeight;

        public Biome[] biomes;
        
        [HideInInspector] public int seed;

        public BiomePosition[] RandomizeBiomePositions()
        {
            System.Random prng = new System.Random(seed);

            List<BiomePosition> createdBiomes = new List<BiomePosition>();
            for (int b = 0; b < biomes.Length; b++)
            {
                for (int i = 0; i < biomes[b].countOfBiomes; i++)
                {
                    Vector2Int position = new Vector2Int(prng.Next(0, mapWidth), prng.Next(0, mapHeight));
                    createdBiomes.Add(new BiomePosition(biomes[b], position));
                }
            }

            return createdBiomes.ToArray();
        }

        public int[,] GenerateBiomeMask(BiomePosition[] targetBiomes)
        {
            int[,] mask = new int[mapWidth, mapHeight];
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    int biomeIndex = 0;
                    int dist = int.MaxValue;

                    for (int b = 0; b < targetBiomes.Length; b++)
                    {
                        Vector2Int biomePosition = targetBiomes[b].position;

                        int xdiff = biomePosition.x - i;
                        int ydiff = biomePosition.y - j;
                        int cdist = xdiff * xdiff + ydiff * ydiff;

                        if (cdist < dist)
                        {
                            biomeIndex = targetBiomes[b].targetBiome.index;
                            dist = cdist;
                        }
                    }
                    mask[i, j] = biomeIndex;
                }
            }
            return mask;
        }

        public float[,] GenerateBiomeMap(int[,] mask)
        {
            // Generate biomes layers
            Dictionary<int, BiomeLayer> indToBiome = new Dictionary<int, BiomeLayer>();
            for (int i = 0; i < biomes.Length; i ++)
            {
                if (!indToBiome.ContainsKey(biomes[i].index))
                {
                    BiomeGenerationSettings settings = biomes[i].settings;
                    float[,] noiseLayer = Noise.GenerateNoiseMap(seed, mapWidth, mapHeight, settings.heightCurve, settings.noiseScale, settings.octaves, settings.persistance, settings.lacunarity, settings.offset);
                    indToBiome[biomes[i].index] = new BiomeLayer(biomes[i], noiseLayer);
                }
            }

            float[,] map = new float[mask.GetLength(0), mask.GetLength(1)];

            // Set layers in map
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (indToBiome.ContainsKey(mask[x,y]))
                    {
                        map[x, y] = indToBiome[mask[x, y]].noiseLayer[x, y];
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
                    colorMap[y * width + x] = indToColor[mask[x, y]];
                }
            }

            return colorMap;
        }
    }

    public struct BiomePosition
    {
        public BiomePosition(Biome b, Vector2Int p)
        {
            targetBiome = b;
            position = p;
        }

        public Biome targetBiome;
        public Vector2Int position;
    }

    public struct BiomeLayer
    {
        public BiomeLayer(Biome b, float[,] nl)
        {
            targetBiome = b;
            noiseLayer = nl;
        }

        public Biome targetBiome;
        public float[,] noiseLayer;
    }
}
