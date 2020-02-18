using UnityEngine;

namespace Generator.Custom
{
    [WorldGenerator(10, true, true, "mapWidth", "mapHeight", "mapGenerationSettings")]
    public class HeightMapGenerator : SubGenerator
    {
        public bool UseFalloffMap;
        public AnimationCurve falloffMapCurve;

        public override void OnGenerate()
        {
            int mapWidth = GetValue<int>("mapWidth");
            int mapHeight = GetValue<int>("mapHeight");
            GenerationSettings settings = GetValue<GenerationSettings>("mapGenerationSettings");

            if (UseFalloffMap)
                GenerateFalloffMap(mapWidth, mapHeight);

            GenerateHeightMap(mapWidth, mapHeight, settings);
            GenerationCompleted();
        }

        public void GenerateFalloffMap(int width, int height)
        {
            values["falloffMap"] = Noise.GenerateFalloffMap(width, height);
        }

        public void GenerateHeightMap(int mapWidth, int mapHeight, GenerationSettings settings)
        {
            float[,] heightMap = Noise.GenerateNoiseMap(ownPrng, mapWidth, mapHeight, settings);

            // Use falloff map offset
            if (TryGetValue("falloffMap", out float[,] falloffMap))
            {
                for (int x = 0; x < heightMap.GetLength(0); x++)
                {
                    for (int y = 0; y < heightMap.GetLength(1); y++)
                    {
                        heightMap[x, y] -= falloffMapCurve.Evaluate(falloffMap[x, y]);
                    }
                }
            }
            
            values["heightMap"] = heightMap;
        }
    }
}

