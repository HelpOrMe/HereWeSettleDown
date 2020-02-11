using UnityEngine;

namespace Generator.Test
{
    public class MapGenerator : MonoBehaviour
    {
        public MapDisplay mapDisplay;

        public int mapWidth;
        public int mapHeight;
        public float noiseScale;

        public int octaves;
        [Range(0f, 1f)]
        public float persistance;
        public float lacunarity;

        public int seed;
        public Vector2 offset;

        public bool autoUpdate;

        public void GenerateMap()
        {
            // Generate new noise map and display it
            float[,] noiseMap = Noise.GenerateNoiseMap(seed, mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, offset);
            Color[] colorMap = GenerateColorMap(noiseMap);
            mapDisplay.DrawColorMap(colorMap, noiseMap.GetLength(0), noiseMap.GetLength(1));
        }

        public Color[] GenerateColorMap(float[,] noiseMap)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }

            return colorMap;
        }

        void OnValidate()
        {
            if (mapWidth < 1) mapWidth = 1;
            if (mapHeight < 1) mapHeight = 1;
            if (lacunarity < 1) lacunarity = 1;
            if (octaves < 0) octaves = 0;
        }
    }
}
