using System.Collections.Generic;
using System.Linq;

namespace Generator.Custom
{
    [WorldGenerator(9, true, true, "mapWidth", "mapHeight", "heightMap")]
    public class BiomesMapGenerator : SubGenerator
    {
        public Biome[] biomes;
        private Dictionary<int, Biome> indToBiome = new Dictionary<int, Biome>();

        public override void OnRegistrate()
        {
            values["biomes"] = biomes;
        }

        public override void OnGenerate()
        {
            int width = GetValue<int>("mapWidth");
            int height = GetValue<int>("mapHeight");

            CreateIndexToBiomeDict();
            GenerateBiomes(width, height);

            GenerationCompleted();
        }

        public void CreateIndexToBiomeDict()
        {
            foreach (Biome biome in biomes)
            {
                if (!indToBiome.ContainsKey(biome.index))
                    indToBiome[biome.index] = biome;
            }
        }

        public void GenerateBiomes(int width, int height)
        {
            float[,] heightMap = GetValue<float[,]>("heightMap");
            float[,] modHeightMap = (float[,])heightMap.Clone();
            BiomeMask[] biomeMasks = new BiomeMask[biomes.Length];
            int[,] globalBiomesMask = new int[width, height];

            for (int i = 0; i < biomes.Length; i ++)
            {
                Biome biome = biomes[i];

                int[,] biomeMask = new int[width, height];
                float[,] biomeHeightMask = Noise.GenerateNoiseMap(ownPrng, width, height, biome.GetBiomeMask());

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (biome.absolute)
                        {
                            globalBiomesMask[x, y] = biome.index;
                            biomeMask[x, y] = biome.index;
                            if (biome.isEvaluteHeight)
                                modHeightMap[x, y] = biome.evaluteHeight.Evaluate(heightMap[x, y]);
                        }

                        // Mask height condition
                        else if (biome.minMaskHeight < biomeHeightMask[x, y] && biome.maxMaskHeight > biomeHeightMask[x, y])
                        {
                            // Spawn on biome condition
                            if (biome.spawnOnAllBiomes || globalBiomesMask[x, y] == 0 ||
                                GetBiomesIndexes(biome.spawnOnBiomes).Contains(globalBiomesMask[x, y]))
                            {
                                // Spawn on height condition
                                if (biome.anySpawnHeight || (biome.minSpawnHeight < heightMap[x, y] && biome.maxSpawnHeight > heightMap[x, y]))
                                {
                                    globalBiomesMask[x, y] = biome.index;
                                    biomeMask[x, y] = biome.index;

                                    // Evalute height condition
                                    if (biome.isEvaluteHeight)
                                    {
                                        modHeightMap[x, y] = biome.evaluteHeight.Evaluate(heightMap[x, y]);
                                    }
                                }
                            }
                        }
                    }
                }
                biomeMasks[i] = new BiomeMask(biomeMask);
            }

            values["modHeightMap"] = modHeightMap;
            values["biomeMasks"] = biomeMasks;
            values["globalBiomeMask"] = globalBiomesMask;
        }

        public int[] GetBiomesIndexes(Biome[] biomes)
        {
            int[] indexes = new int[biomes.Length];
            for (int i = 0; i < biomes.Length; i++)
                indexes[i] = biomes[i].index;
            return indexes;
        }

        public void GenerateGlobalMask(int width, int height)
        {
            BiomeMask[] biomeMasks = GetValue<BiomeMask[]>("biomeMasks");

            int[,] globalMask = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < biomeMasks.Length; i++)
                    {
                        if (biomeMasks[i].mask[x, y] != 0)
                            globalMask[x, y] = biomeMasks[i].mask[x, y];
                    }
                }
            }

            values["globalMask"] = globalMask;
        }
    }

    public struct BiomeMask
    {
        public int[,] mask;

        public BiomeMask(int[,] mask)
        {
            this.mask = mask;
        }
    }
}
