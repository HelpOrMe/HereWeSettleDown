using System.Collections.Generic;
using UnityEngine;
using World.Generator.HeightMap;
using World.Generator.Helper;

namespace World.Generator.Biomes
{
    [CustomGenerator(true, "mapWidth", "mapHeight")]
    public class BiomesGenerator : SubGenerator
    {
        public Biome[] biomes;

        public override void OnGenerate()
        {
            GenerateBiomes();
            GenerationCompleted();
        }

        private void GenerateBiomes()
        {
            int width = GetValue<int>("mapWidth");
            int height = GetValue<int>("mapHeight");

            float[,] heightMap = new float[width, height];
            int[,] globalBiomesMask = new int[width, height];
            BiomeMask[] biomeMasks = new BiomeMask[biomes.Length];

            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i] != null)
                {
                    biomes[i].index = i + 1;
                    if (biomes[i].GetNewHeightMap(ownPrng, biomes, ref heightMap, ref globalBiomesMask))
                    {
                        biomeMasks[i] = new BiomeMask(biomes[i].GetBiomeMask(), biomes[i]);
                    }
                }
            }

            values["heightMap"] = heightMap;
            values["biomeMasks"] = biomeMasks;
            values["globalBiomeMask"] = globalBiomesMask;
        }
    }

    public struct BiomeMask
    {
        public float[,] mask;
        public Biome biome;

        public BiomeMask(float[,] mask, Biome biome)
        {
            this.mask = mask;
            this.biome = biome;
        }
    }
}
