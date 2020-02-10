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
            mapDisplay.DrawNoiseMap(noiseMap);
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
