using UnityEngine;

namespace World.Generator.HeightMap
{
    [CustomGenerator(10, true, "mapWidth", "mapHeight", "mapNoiseSettings")]
    public class HeightMapGenerator : SubGenerator
    {
        public NoiseSettings heightMapSettings;

        public bool UseFalloffMap;
        public AnimationCurve falloffMapCurve;

        public override void OnGenerate()
        {
            int mapWidth = GetValue<int>("mapWidth");
            int mapHeight = GetValue<int>("mapHeight");

            if (UseFalloffMap)
                GenerateFalloffMap(mapWidth, mapHeight);

            GenerateHeightMap(mapWidth, mapHeight);
            GenerationCompleted();
        }

        public void GenerateFalloffMap(int width, int height)
        {
            values["falloffMap"] = Noise.GenerateFalloffMap(width, height);
        }

        public void GenerateHeightMap(int mapWidth, int mapHeight)
        {
            float[,] heightMap = Noise.GenerateNoiseMap(ownPrng, mapWidth, mapHeight, );

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

