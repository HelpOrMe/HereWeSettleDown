using System.Linq;
using UnityEngine;
using World.Generator.HeightMap;
using World.Generator.Helper;

namespace World.Generator.Biomes
{
    public class BiomePattern : Biome
    {
        public bool SpawnOnAllBiomes;
        public Biome[] spawnOnBiomes;

        public bool UseDefaultHeightMap;
        public NoiseSettings noiseSettings;
        public AnimationCurve worldHeightCurve;
        public HeightMaskPattern heightMaskPattern;

        private float[,] biomeMask;

        public override bool GetNewHeightMap(System.Random prng, Biome[] biomes, ref float[,] currentHeightMap, ref int[,] globalBiomesMask)
        {
            int width = currentHeightMap.GetLength(0);
            int height = currentHeightMap.GetLength(1);

            float[,] targetHeightMap = UseDefaultHeightMap ? currentHeightMap : Noise.GenerateNoiseMap(prng, width, height, noiseSettings);
            biomeMask = heightMaskPattern.GetMask(prng, targetHeightMap);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (biomeMask[x, y] > 0)
                    {
                        if (SpawnOnAllBiomes ||
                           spawnOnBiomes.Contains(biomes[globalBiomesMask[x, y] - 1]))
                        {
                            globalBiomesMask[x, y] = index;
                            float targetHeight = UseDefaultHeightMap ? worldHeightCurve.Evaluate(targetHeightMap[x, y]) : targetHeightMap[x, y];
                            currentHeightMap[x, y] = Mathf.Lerp(currentHeightMap[x, y], targetHeight, biomeMask[x, y] * (1 + power));
                        }
                    }
                }
            }

            return true;
        }

        public override float[,] GetBiomeMask()
        {
            return biomeMask;
        }
    }
}