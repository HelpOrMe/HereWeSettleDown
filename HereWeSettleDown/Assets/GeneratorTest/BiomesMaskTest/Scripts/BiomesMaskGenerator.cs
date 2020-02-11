using System.Collections.Generic;
using UnityEngine;

namespace Generator.Test.BiomeMask
{
    public class BiomesMaskGenerator : MonoBehaviour
    {
        public MapDisplay mapDisplay;

        public int mapWidth;
        public int mapHeight;

        public Biome[] biomes;

        public int seed;
        public bool autoUpdate;

        public void GenerateMap()
        {
            BiomeAreaInfo[] biomes = RandomizeBiomePositions();
            int[,] map = GenerateBiomeMap(biomes);
            Color[] colorMap = CreateColorMap(map);
            mapDisplay.DrawColorMap(colorMap, mapWidth, mapHeight);
        }

        public BiomeAreaInfo[] RandomizeBiomePositions()
        {
            System.Random prng = new System.Random(seed);

            List<BiomeAreaInfo> createdBiomes = new List<BiomeAreaInfo>();
            for (int b = 0; b < biomes.Length; b++)
            {
                for (int i = 0; i < biomes[b].countOfBiomes; i ++)
                {
                    Vector2Int position = new Vector2Int(prng.Next(0, mapWidth), prng.Next(0, mapHeight));
                    createdBiomes.Add(new BiomeAreaInfo(biomes[b], position));
                }
            }

            return createdBiomes.ToArray();
        }

        public int[,] GenerateBiomeMap(BiomeAreaInfo[] targetBiomes)
        {
            int[,] map = new int[mapWidth, mapHeight];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
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
                            biomeIndex = targetBiomes[b].targetBiome.biomeIndex;
                            dist = cdist;
                        }
                    }
                    map[i,j] = biomeIndex;
                }
            }
            return map;
        }

        public Color[] CreateColorMap(int[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            // Set dicitionary with index to colors
            Dictionary<int, Color> indToColor = new Dictionary<int, Color>();
            for (int i = 0; i < biomes.Length; i++)
                indToColor.Add(biomes[i].biomeIndex, biomes[i].color);

            // Create color map
            Color[] colorMap = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    colorMap[y * width + x] = indToColor[map[x, y]];
                }
            }

            return colorMap;
        }
    }

    [System.Serializable]
    public struct Biome
    {
        public string name;
        public Color color;
        public int biomeIndex;
        public int countOfBiomes;
    }

    public struct BiomeAreaInfo
    {
        public BiomeAreaInfo(Biome b, Vector2Int p)
        {
            targetBiome = b;
            position = p;
        }

        public Biome targetBiome;
        public Vector2Int position;
    }
}