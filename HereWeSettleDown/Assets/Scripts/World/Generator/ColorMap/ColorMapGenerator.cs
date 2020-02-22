using System.Collections.Generic;
using UnityEngine;
using World.Generator.Biomes;
using World.Chunks;

namespace World.Generator.ColorMap
{
    [CustomGenerator(true, "heightMap", "biomeMasks", "globalBiomeMask")]
    public class ColorMapGenerator : SubGenerator
    {
        public DisplayType displayType;
        [Range(0, 3)] public int smoothIterations;

        public override void OnGenerate()
        {
            GenerateColorPackMap(GenerateSimpleColorMap());
            GenerationCompleted();
        }

        private Color[,] GenerateSimpleColorMap()
        {
            float[,] heightMap = GetValue<float[,]>("heightMap");
            BiomeMask[] biomeMasks = GetValue<BiomeMask[]>("biomeMasks");
            int[,] globalBiomeMask = GetValue<int[,]>("globalBiomeMask");

            Color[,] colorMap = null;

            switch (displayType)
            {
                case (DisplayType.GameView):
                    colorMap = ColorMapFromColorRegions(heightMap, biomeMasks);
                    break;

                case (DisplayType.Falloff):
                    if (TryGetValue("falloffMap", out float[,] falloffMap))
                        colorMap = ColorMapFromHeightMap(falloffMap);
                    break;

                case (DisplayType.Biomes):
                    colorMap = ColorMapFromBiomes(globalBiomeMask, biomeMasks);
                    break;
            }

            if (colorMap == null)
                colorMap = ColorMapFromHeightMap(heightMap);

            return colorMap;
        }

        private void GenerateColorPackMap(Color[,] colorMap)
        {
            int width = GetValue<int>("mapWidth");
            int height = GetValue<int>("mapHeight");

            ColorPack[,] convertedColorMap = ConvertColorMap(width, height, colorMap);
            for (int i = 0; i < smoothIterations; i++)
                convertedColorMap = SmoothColorMap(convertedColorMap);

            values["colorMap"] = convertedColorMap;
        }

        private ColorPack[,] ConvertColorMap(int width, int height, Color[,] colors)
        {
            ColorPack[,] colorMap = EmptyColorMap(width - 1, height - 1);

            for (int x = 0; x < width; x ++)
            {
                for (int y = 0; y < height; y ++)
                {
                    Color targetColor = colors[x, y];

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

        private ColorPack[,] SmoothColorMap(ColorPack[,] colorMap)
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

        private ColorPack[,] EmptyColorMap(int width, int height)
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

        private Color[,] ColorMapFromHeightMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Color[,] colorMap = new Color[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorMap[x, y] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                }
            }

            return colorMap;
        }

        private Color[,] ColorMapFromColorRegions(float[,] heightMap, BiomeMask[] biomeMasks)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Color[,] colorMap = new Color[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < biomeMasks.Length; i++)
                    {
                        if (biomeMasks[i].mask != null && biomeMasks[i].mask[x, y] > 0)
                        {
                            if (biomeMasks[i].biome.OverrideColors ||
                                colorMap[x, y] == default)
                            {
                                foreach (BiomeColorRegion colorRegion in biomeMasks[i].biome.colorRegions)
                                {
                                    if (heightMap[x, y] <= colorRegion.height)
                                    {
                                        colorMap[x, y] = colorRegion.color;
                                        break;
                                    }
                                }
                            }
                        }
                        
                    }
                }
            }

            return colorMap;
        }

        private Color[,] ColorMapFromBiomes(int[,] globalBiomeMask, BiomeMask[] biomeMasks)
        {
            int width = globalBiomeMask.GetLength(0);
            int height = globalBiomeMask.GetLength(1);

            // Create color map
            Color[,] colorMap = new Color[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = globalBiomeMask[x, y] - 1;
                    colorMap[x, y] = Color.Lerp(Color.black, biomeMasks[index].biome.mapColor, biomeMasks[index].mask[x, y] * (1 - biomeMasks[index].biome.power));
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
