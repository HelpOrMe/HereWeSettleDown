using UnityEngine;

namespace World.Generator.Helper
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(System.Random prng, int mapWidth, int mapHeight, NoiseSettings settings)
        {
            float[,] heightMap = GenerateNoiseMap(prng, mapWidth, mapHeight, settings.noiseScale, settings.octaves, settings.persistance, settings.lacunarity, settings.offset);

            // Evalute heights by settings curve
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    heightMap[x, y] = settings.heightCurve.Evaluate(heightMap[x, y]);
                }
            }
            return heightMap;
        }

        public static float[,] GenerateNoiseMap(System.Random prng, int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0)
            {
                scale = 0.0001f;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;


            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {

                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }

        public static float[,] GenerateFalloffMap(int width, int height)
        {
            float[,] map = new float[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float x = i / (float)width * 2 - 1;
                    float y = j / (float)height * 2 - 1;

                    float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    map[i, j] = EvaluateFalloffValue(value);
                }
            }

            return map;
        }

        static float EvaluateFalloffValue(float value)
        {
            float a = 3;
            float b = 2.2f;

            return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
        }
    }

    [System.Serializable]
    public struct NoiseSettings
    {
        public AnimationCurve heightCurve;
        public float noiseScale;
        public Vector2 offset;

        public int octaves;
        [Range(0f, 1f)] public float persistance;
        public float lacunarity;
    }
}
