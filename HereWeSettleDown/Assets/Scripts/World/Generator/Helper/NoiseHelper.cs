using UnityEngine;
using AccidentalNoise;
using Helper.Random;

namespace World.Generator.Helper
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(System.Random prng, int mapWidth, int mapHeight, AccidentalNoiseSettings settings)
        {
            ModuleBase moduleBase = settings.GetFractal(prng.Next(int.MinValue, int.MaxValue));
            SMappingRanges ranges = new SMappingRanges();

            float[,] noiseMap = new float[mapWidth, mapHeight];
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    double p = x / (double)mapWidth;
                    double q = y / (double)mapHeight;

                    double nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
                    double ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

                    noiseMap[x, y] = (float)moduleBase.Get(nx * settings.scale, ny * settings.scale);
                }
            }

            return noiseMap;
        }

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
    public class NoiseSettings
    {
        public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 5, 5);

        public float noiseScale = 25;
        public int octaves = 4;
        [Range(0f, 1f)] public float persistance = 0.5f;
        public float lacunarity = 2;

        public Vector2 offset = Vector2.zero;
    }

    [System.Serializable]
    public class AccidentalNoiseSettings
    {
        public FractalType fractalType = FractalType.FBM;
        public BasisTypes basisType = BasisTypes.GRADIENT;
        public InterpTypes interpType = InterpTypes.QUINTIC;

        public float scale = 1;
        public int octaves = 6;
        public float frequency = 2f;
        public float lacunarity = 2f;

        public ModuleBase GetFractal(int seed)
        {
            Fractal ground_shape_fractal = new Fractal(fractalType, basisType, interpType, octaves, frequency, (uint)seed);
            ground_shape_fractal.SetLacunarity(lacunarity);
            return ground_shape_fractal as ModuleBase;
        }
    }
}
