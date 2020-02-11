using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public static class TextureGenerator
    {
        public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            texture.SetPixels(colorMap);
            texture.Apply();

            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Color[] colourMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                }
            }

            return TextureFromColorMap(colourMap, width, height);
        }

        public static Texture2D TextureFromColorRegions(float[,] map, int[,] mask, Biome[] biomes)
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

            return TextureFromColorMap(colorMap, width, height);
        }
    }
}
