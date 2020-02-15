using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generator.Custom
{
    [WorldGenerator(10, "mapWidth", "mapHeight", "mapGenerationSettings")]
    public class HeightMapGenerator : SubGenerator
    {
        public bool UseFalloffMap;
        public AnimationCurve falloffMapCurve;

        // public float heightScale;
        // public float maxFinalHeight;
        // public float minFinalHeight;

        // From mapGenerationSettings
        private AnimationCurve heightCurve;

        private static Dictionary<int, List<Func<float[,], int, int, float>>> registratedFunctions = 
            new Dictionary<int, List<Func<float[,], int, int, float>>>();

        public static void RegistrateFunc(Func<float[,], int, int, float> func, int priority)
        {
            // float[,] map, int x, int y
            if (!registratedFunctions.ContainsKey(priority))
                registratedFunctions[priority] = new List<Func<float[,], int, int, float>>();
            registratedFunctions[priority].Add(func);
        }

        public override void OnGenerate()
        {
            int mapWidth = GetValue<int>("mapWidth");
            int mapHeight = GetValue<int>("mapHeight");
            GenerationSettings settings = GetValue<GenerationSettings>("mapGenerationSettings");

            if (UseFalloffMap)
            {
                GenerateFalloffMap(mapWidth, mapHeight);
                RegistrateFunc(EvaluteHeightByFalloffMap, -1);
            }

            // Registrate animation curve from generation settings
            heightCurve = settings.heightCurve;
            RegistrateFunc(EvaluteHeightByCurve, 0);

            GenerateHeightMap(mapWidth, mapHeight, settings);
            GenerationCompleted();
        }

        public void GenerateHeightMap(int mapWidth, int mapHeight, GenerationSettings settings)
        {
            float[,] heightMap = Noise.GenerateNoiseMap(
                mapWidth, mapHeight, settings.noiseScale, 
                settings.octaves, settings.persistance, 
                settings.lacunarity, settings.offset);

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    // Invoke all registrated methods

                    int[] keys = registratedFunctions.Keys.ToArray();
                    Array.Sort(keys);

                    foreach (int priority in keys)
                    {
                        for (int i = 0; i < registratedFunctions[priority].Count; i ++)
                        {
                            heightMap[x,y] = registratedFunctions[priority][i].Invoke(heightMap, x, y);
                        }
                    }
                }
            }
            values["heightMap"] = heightMap;
        }

        public void GenerateFalloffMap(int width, int height)
        {
            values["falloffMap"] = Noise.GenerateFalloffMap(width, height);
        }

        public float EvaluteHeightByCurve(float[,] map, int x, int y)
        {
            return heightCurve.Evaluate(map[x, y]);
        }

        public float EvaluteHeightByFalloffMap(float[,] map, int x, int y)
        {
            return map[x, y] - falloffMapCurve.Evaluate(GetValue<float[,]>("falloffMap")[x, y]);
        }
    }
}

