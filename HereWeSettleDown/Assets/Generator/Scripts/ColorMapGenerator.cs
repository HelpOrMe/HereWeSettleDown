using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public static class ColorMapGenerator
    {
        public static Color[] ColorMapFromHeightMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                }
            }

            return colorMap;
        }

        public static Color[] ColorMapFromColorRegions(float[,] map, int[,] mask, Biome[] biomes)
        {
            // Set dicitionary with index to colors
            Dictionary<int, BiomeColorRegion[]> indexToColorRegions = new Dictionary<int, BiomeColorRegion[]>();
            for (int i = 0; i < biomes.Length; i++)
                if (!indexToColorRegions.ContainsKey(biomes[i].index))
                    indexToColorRegions.Add(biomes[i].index, biomes[i].colorRegions);

            // Create color map
            int width = mask.GetLength(0);
            int height = mask.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (indexToColorRegions.ContainsKey(mask[x,y]))
                    {
                        foreach (BiomeColorRegion colorRegion in indexToColorRegions[mask[x, y]])
                        {
                            if (map[x, y] <= colorRegion.height)
                            {
                                colorMap[y * width + x] = colorRegion.color;
                                break;
                            }
                        }
                    }
                }
            }

            return colorMap;
        }

        public static Color[] Gradient(int width, int height)
        {
            Color[] colorMap = new Color[width * height];
            for (int i = 0; i < colorMap.Length; i++)
                colorMap[i] = Color.Lerp(Color.black, Color.white, i / (float)colorMap.Length);
            return colorMap;
        }
    }
}
