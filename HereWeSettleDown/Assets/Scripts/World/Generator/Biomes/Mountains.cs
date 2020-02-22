using System.Collections.Generic;
using UnityEngine;
using World.Generator.Helper;

namespace World.Generator.Biomes
{
    public class Mountains : Biome
    {
        public AccidentalNoiseSettings settings;
        public int heightScale = 1;

        public float minPicksDetection;
        public float maxPicksDetection;

        public float minWorldHeight;
        public float maxWorldHeight;

        public int minPicksCount = 1;
        public int maxPicksCount = 4;

        private float[,] biomeMask;

        public override bool GetNewHeightMap(System.Random prng, Biome[] biomes, ref float[,] currentHeightMap, ref int[,] globalBiomesMask)
        {
            int width = currentHeightMap.GetLength(0);
            int height = currentHeightMap.GetLength(1);

            float[,] mountainsHeightMap = Noise.GenerateNoiseMap(prng, width, height, settings);
            biomeMask = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float fadeValue = GetNoiseFadeValue(Mathf.Clamp01(mountainsHeightMap[x, y]), 0, 1, false) * (1 + power);
                    float heightValue = Mathf.Clamp01(mountainsHeightMap[x, y]) * heightScale;
                    currentHeightMap[x, y] = Mathf.Lerp(currentHeightMap[x, y], heightValue, fadeValue);
                    biomeMask[x, y] = fadeValue;
                    globalBiomesMask[x, y] = index;
                }
            }

            return true;
        }

        private float GetNoiseFadeValue(float value, float minHeight, float maxHeight, bool reverse)
        {
            float fadeValue = (value - minHeight) / (maxHeight - minHeight);
            return reverse ? 1 - fadeValue : fadeValue;
        }

        public override float[,] GetBiomeMask()
        {
            return biomeMask;
        }
    }
}

