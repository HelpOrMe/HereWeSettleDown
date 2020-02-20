using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Generator.Helper;
using World.Generator.HeightMap;

namespace World.Generator.Biomes
{
    [CustomGenerator(9, true, typeof(HeightMapGenerator))]
    public class BiomesGenerator : SubGenerator
    {
        public Biome[] biomes;
        private readonly Dictionary<int, Biome> indToBiome = new Dictionary<int, Biome>();

        public override void OnRegistrate()
        {
            values["biomes"] = biomes;
        }

        public override void OnGenerate()
        {
            CreateIndexToBiomeDict();
            GenerateBiomes();
            GenerationCompleted();
        }

        private void CreateIndexToBiomeDict()
        {
            foreach (Biome biome in biomes)
            {
                if (!indToBiome.ContainsKey(biome.index))
                    indToBiome[biome.index] = biome;
            }
        }

        private void GenerateBiomes()
        {
            float[,] heightMap = GetValue<float[,]>("heightMap");
            float[,] biomedHeightMap = (float[,])heightMap.Clone();

            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            BiomeMask[] biomeMasks = new BiomeMask[biomes.Length];
            int[,] globalBiomesMask = new int[width, height];

            for (int i = 0; i < biomes.Length; i++)
            {
                biomes[i].index = i;
                Biome biome = biomes[i];

                if (biome.heightMaskPattern == null)
                    continue;

                float[,] biomeHeightMap = Noise.GenerateNoiseMap(ownPrng, width, height, biome.noiseSettings);
                float[,] biomeMask = biome.heightMaskPattern.GetMask(ownPrng, biome.UseDefaultHeightMap ? heightMap : biomeHeightMap);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (biomeMask[x, y] > 0)
                        {
                            if (biome.SpawnOnAllBiomes || 
                                GetBiomesIndexes(biome.spawnOnBiomes).Contains(globalBiomesMask[x, y]))
                            {
                                globalBiomesMask[x, y] = biome.index;
                                if (biome.UseDefaultHeightMap)
                                    biomedHeightMap[x, y] = biome.worldHeightCurve.Evaluate(heightMap[x, y]);
                                else
                                    biomedHeightMap[x, y] = Mathf.Lerp(biomedHeightMap[x, y], biomeHeightMap[x, y], biomeMask[x, y] * (1 + biome.power));
                            }
                        }
                    }
                }
                biomeMasks[i] = new BiomeMask(biomeMask, biome);
                Debug.Log(biome.GetType().Name + " generated.");
            }

            values["biomedHeightMap"] = biomedHeightMap;
            values["biomeMasks"] = biomeMasks;
            values["globalBiomeMask"] = globalBiomesMask;
        }

        private int[] GetBiomesIndexes(Biome[] biomes)
        {
            int[] indexes = new int[biomes.Length];
            for (int i = 0; i < biomes.Length; i++)
                indexes[i] = biomes[i].index;
            return indexes;
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
