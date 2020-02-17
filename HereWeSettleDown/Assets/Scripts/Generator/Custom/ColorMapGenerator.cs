﻿using System.Collections.Generic;
using UnityEngine;
using World.MeshSystem;

namespace Generator.Custom
{
    [WorldGenerator(8, true, false, "heightMap", "biomes", "biomeMasks", "globalMask")]
    public class ColorMapGenerator : SubGenerator
    {
        public DisplayType displayType;
        [Range(0, 3)] 
        public int smoothIterations;

        private Color[] colorMap;

        public override void OnGenerate()
        {
            GenerateSimpleColorMap();
            GeneratePackColorMap();
            GenerationCompleted();
        }

        public void GenerateSimpleColorMap()
        {
            float[,] heightMap = GetValue<float[,]>("heightMap");

            if (DisplayType.GameView == displayType)
            {
                Biome[] biomes = GetValue<Biome[]>("biomes");
                BiomeMask[] biomeMasks = GetValue<BiomeMask[]>("biomeMasks");

                colorMap = ColorMapFromColorRegions(heightMap, biomeMasks, biomes);
            }

            else if (DisplayType.Falloff == displayType)
            {
                if (TryGetValue("falloffMap", out float[,] falloffMap))
                {
                    colorMap = ColorMapFromHeightMap(falloffMap);
                }
                else
                {
                    colorMap = ColorMapFromHeightMap(heightMap);
                }
            }

            else if (DisplayType.Biomes == displayType)
            {
                int[,] globalMask = GetValue<int[,]>("globalMask");
                Biome[] biomes = GetValue<Biome[]>("biomes");

                colorMap = ColorMapFromBiomes(globalMask, biomes);
            }

            else // Noise
            {
                colorMap = ColorMapFromHeightMap(heightMap);
            }
        }

        public void GeneratePackColorMap()
        {
            int width = GetValue<int>("mapWidth");
            int height = GetValue<int>("mapHeight");

            ColorPack[,] convertedColorMap = ConvertColorMap(width, height, colorMap);
            for (int i = 0; i < smoothIterations; i++)
                convertedColorMap = SmoothColorMap(convertedColorMap);

            values["colorMap"] = convertedColorMap;
        }

        public ColorPack[,] ConvertColorMap(int width, int height, Color[] _colors)
        {
            ColorPack[,] colorMap = EmptyColorMap(width - 1, height - 1);

            for (int x = 0; x < width; x ++)
            {
                for (int y = 0; y < height; y ++)
                {
                    Color targetColor = _colors[width * y + x];

                    // Color main quad
                    if (x < width - 1 && y < height - 1)
                        colorMap[x, y][0] = targetColor;

                    // Color left-bottom quad
                    if (x - 1 >= 0 && y - 1 > 0)
                        colorMap[x - 1, y - 1][1] = targetColor;

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

        public ColorPack[,] SmoothColorMap(ColorPack[,] colorMap)
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
                        neighborColors.Add(colorPack[i + sign]);
                        if (x - sign >= 0 && x - sign < width)
                            neighborColors.Add(colorMap[x - sign, y][i + sign]);
                        if (y - sign >= 0 && y - sign < height)
                            neighborColors.Add(colorMap[x, y - sign][i + sign]);

                        if (TwoSameColors(neighborColors.ToArray(), out Color newColor))
                            colorPack[i] = newColor;
                    }
                    colorMap[x, y] = colorPack;
                }
            }
            return colorMap;
        }

        private bool TwoSameColors(Color[] colors, out Color targetColor)
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

        public ColorPack[,] EmptyColorMap(int width, int height)
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

        public Color[] ColorMapFromHeightMap(float[,] heightMap)
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

        public Color[] ColorMapFromColorRegions(float[,] heightMap, BiomeMask[] biomeMasks, Biome[] biomes)
        {
            // Set dicitionary with index to colors
            Dictionary<int, BiomeColorRegion[]> indexToColorRegions = new Dictionary<int, BiomeColorRegion[]>();
            for (int i = 0; i < biomes.Length; i++)
                if (!indexToColorRegions.ContainsKey(biomes[i].index))
                    indexToColorRegions.Add(biomes[i].index, biomes[i].colorRegions);

            // Create color map
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < biomeMasks.Length; i ++)
                    {
                        if (indexToColorRegions.ContainsKey(biomeMasks[i].mask[x, y]))
                        {
                            foreach (BiomeColorRegion colorRegion in indexToColorRegions[biomeMasks[i].mask[x, y]])
                            {
                                if (heightMap[x, y] <= colorRegion.height)
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

        public Color[] ColorMapFromBiomes(int[,] globalMask, Biome[] biomes)
        {
            int width = globalMask.GetLength(0);
            int height = globalMask.GetLength(1);

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
                    if (indToColor.ContainsKey(globalMask[x, y]))
                        colorMap[y * width + x] = indToColor[globalMask[x, y]];
                }
            }

            return colorMap;
        }

        public enum DisplayType
        {
            Noise,
            Biomes,
            Falloff,
            GameView
        }
    }
}
