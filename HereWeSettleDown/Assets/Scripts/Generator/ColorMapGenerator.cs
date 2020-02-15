using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public static class ColorMapGenerator
    {
        public static ColorPack[,] ConvertColorMap(int width, int height, Color[] _colors)
        {
            ColorPack[,] colorMap = EmptyColorMap(width - 1, height - 1);

            for (int x = 0; x < width; x ++)
            {
                for (int y = 0; y < height; y ++)
                {
                    Color targetColor = _colors[width * y + x];
                    Color[] colorList = new Color[3] { targetColor, targetColor, targetColor };

                    // Color main quad
                    if (x < width - 1 && y < height - 1)
                        colorMap[x, y][0] = colorList;

                    // Color left-bottom quad
                    if (x - 1 >= 0 && y - 1 > 0)
                        colorMap[x - 1, y - 1][1] = colorList;

                    // Color left quad
                    if (x - 1 >= 0 && y < height - 1)
                        colorMap[x - 1, y] = new ColorPack(targetColor, targetColor);

                    // Color bottom quad
                    if (y - 1 >= 0 && x < width - 1)
                        colorMap[x, y - 1] = new ColorPack(targetColor, targetColor);
                }
            }

            return colorMap;
        }

        public static ColorPack[,] SmoothColorMap(ColorPack[,] colorMap)
        {
            int width = colorMap.GetLength(0);
            int height = colorMap.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    ColorPack colorPack = colorMap[x, y];
                    for (int i = 0; i < 2; i ++)
                    {
                        int sign = i == 0 ? 1 : -1;
                        List<Color> neighborColors = new List<Color>();

                        // First triangle
                        neighborColors.Add(colorPack[i + sign][0]);
                        if (x - sign >= 0 && x - sign < width)
                            neighborColors.Add(colorMap[x - sign, y][i + sign][0]);
                        if (y - sign >= 0 && y - sign < height)
                            neighborColors.Add(colorMap[x, y - sign][i + sign][0]);

                        if (TwoSameColors(neighborColors.ToArray(), out Color newColor))
                            colorPack[i] = new Color[] { newColor, newColor, newColor };
                    }
                    colorMap[x, y] = colorPack;
                }
            }
            return colorMap;
        }

        static bool TwoSameColors(Color[] colors, out Color targetColor)
        {
            for (int i = 0; i < colors.Length; i ++)
            {
                for (int j = i + 1; j < colors.Length; j ++)
                {
                    if (colors[i] == colors[j])
                    {
                        targetColor = colors[i];
                        return true;
                    }
                }
            }
            targetColor = Color.black;
            return false;
        }

        public static ColorPack[,] EmptyColorMap(int width, int height)
        {
            ColorPack[,] colorMap = new ColorPack[width , height];

            // Create empty colorPack map
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    colorMap[x, y] = new ColorPack();
                }
            }

            return colorMap;
        }

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

        public static Color[] ColorMapFromColorRegions(float[,] map, BiomeMask[] masks, Biome[] biomes)
        {
            // Set dicitionary with index to colors
            Dictionary<int, BiomeColorRegion[]> indexToColorRegions = new Dictionary<int, BiomeColorRegion[]>();
            for (int i = 0; i < biomes.Length; i++)
                if (!indexToColorRegions.ContainsKey(biomes[i].index))
                    indexToColorRegions.Add(biomes[i].index, biomes[i].colorRegions);

            // Create color map
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < masks.Length; i ++)
                    {
                        if (indexToColorRegions.ContainsKey(masks[i].mask[x, y]))
                        {
                            foreach (BiomeColorRegion colorRegion in indexToColorRegions[masks[i].mask[x, y]])
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
            }

            return colorMap;
        }

        public static Color[] ColorMapFromColorRegions(float[,] map, int[,] globalMask, Biome[] biomes)
        {
            // Set dicitionary with index to colors
            Dictionary<int, BiomeColorRegion[]> indexToColorRegions = new Dictionary<int, BiomeColorRegion[]>();
            for (int i = 0; i < biomes.Length; i++)
                if (!indexToColorRegions.ContainsKey(biomes[i].index))
                    indexToColorRegions.Add(biomes[i].index, biomes[i].colorRegions);

            // Create color map
            int width = globalMask.GetLength(0);
            int height = globalMask.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (indexToColorRegions.ContainsKey(globalMask[x, y]))
                    {
                        foreach (BiomeColorRegion colorRegion in indexToColorRegions[globalMask[x, y]])
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
    }
}
