using UnityEngine;

namespace Generator
{
    public class MapGenerator : MonoBehaviour
    {
        public int seed;

        public int mapWidth;
        public int mapHeight;
        
        public GenerationSettings settings;
        public AnimationCurve meshHeightCurve;
        public float meshHeightMultiplier;

        public bool EvoluteHeight;
        public DisplayType displayType;

        public MapDisplay mapDisplay;
        public BiomesMapGenerator biomesGenerator;

        [HideInInspector] static public float[,] heightMap;
        [HideInInspector] static public int[,] biomesMask;

        public void GenerateMap()
        {
            CorrectValuesInGenerators();
            GenerateGlobalMap();
            biomesMask = biomesGenerator.GenerateBiomeMask();
            if (EvoluteHeight)
                heightMap = biomesGenerator.EvoluteHeightByBiomes(heightMap, biomesMask);

            DisplayMap();
        }

        void CorrectValuesInGenerators()
        {
            biomesGenerator.mapWidth = mapWidth;
            biomesGenerator.mapHeight = mapHeight;
            biomesGenerator.seed = seed;
        }

        void GenerateGlobalMap()
        {
            heightMap = Noise.GenerateNoiseMap(
                seed, mapWidth, mapHeight, settings.noiseScale,
                settings.octaves, settings.persistance,
                settings.lacunarity, settings.offset);
        }

        void DisplayMap()
        {
            Color[] colorMap;

            if (displayType == DisplayType.GameView)
                colorMap = ColorMapGenerator.ColorMapFromColorRegions(heightMap, biomesMask, biomesGenerator.biomes);
            else if (displayType == DisplayType.Biomes)
                colorMap = biomesGenerator.CreateColorMap(biomesMask);
            else if (displayType == DisplayType.Gradeint)
                colorMap = ColorMapGenerator.Gradient(mapWidth, mapHeight);
            else
                colorMap = ColorMapGenerator.ColorMapFromHeightMap(heightMap);

            MeshData meshData = MeshGenerator.GenerateTerrainMesh(seed, heightMap, meshHeightMultiplier, meshHeightCurve);
            meshData.SetColorMap(colorMap);

            mapDisplay.DrawMesh(meshData);
        }

        public enum DisplayType
        {
            Noise,
            Biomes,
            Gradeint,
            GameView
        }
    }

    [System.Serializable]
    public struct GenerationSettings
    {
        public float noiseScale;
        public int octaves;
        [Range(0f, 1f)]
        public float persistance;
        public float lacunarity;
        public Vector2 offset;
    }
}

