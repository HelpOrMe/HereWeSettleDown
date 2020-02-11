using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class BiomesMapGenerator : MonoBehaviour
    {
        public Biome[] biomes;

        [HideInInspector] public int mapWidth;
        [HideInInspector] public int mapHeight;
        [HideInInspector] public int seed;

        public int[,] GenerateBiomeMask()
        {
            BiomePosition[] biomePositions = RandomizeBiomePositions();
            return GenerateMaskByPositions(biomePositions);
        }

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

        public int[,] GenerateMaskByPositions(BiomePosition[] biomesPositions)
        {
            int[,] mask = new int[mapWidth, mapHeight];
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    int biomeIndex = 0;
                    int dist = int.MaxValue;

                    for (int b = 0; b < biomesPositions.Length; b++)
                    {
                        Vector2Int biomePosition = biomesPositions[b].position;

                        int xdiff = biomePosition.x - i;
                        int ydiff = biomePosition.y - j;
                        int cdist = xdiff * xdiff + ydiff * ydiff;

                        if (cdist < dist)
                        {
                            biomeIndex = b;
                            dist = cdist;
                        }
                    }

                    mask[i, j] = biomesPositions[biomeIndex].targetBiome.index;
                }
            }
            return mask;
        }
        
        public float[,] EvoluteHeightByBiomes(float[,] map, int[,] mask)
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
                    if (indToBiome.ContainsKey(mask[x,y]))
                    {
                        map[x, y] = Mathf.Clamp01(indToBiome[mask[x, y]].heightCurve.Evaluate(map[x, y]));
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
}
