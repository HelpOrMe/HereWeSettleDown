using UnityEngine;

namespace Generator
{
    public class MapGenerator : MonoBehaviour
    {
        public int seed;

        public int mapWidth;
        public int mapHeight;

        public Vector2 triangleSizeScale;
        public Vector2Int triangleRangeXScale;
        public Vector2Int triangleRangeYScale;

        public GenerationSettings settings;
        public AnimationCurve meshHeightCurve;
        public float meshHeightMultiplier;
        public bool evaluteHeightFromBiomes;

        [Range(0, 3)]
        public int SmoothIterations;
        public bool FastBiomeColoring;

        public DisplayType displayType;

        public MapDisplay mapDisplay;
        public BiomesMapGenerator biomesGenerator;

        [HideInInspector] static public float[,] heightMap;
        [HideInInspector] static public BiomeMask[] biomesMasks;
        [HideInInspector] static public int[,] globalBiomeMask;

        public void GenerateMap()
        {
            Noise.SetupPRNG(seed);

            GenerateGlobalMap();
            biomesMasks = biomesGenerator.GenerateBiomesMask(mapWidth, mapHeight);
            globalBiomeMask = biomesGenerator.СombineMasks(mapWidth, mapHeight, biomesMasks);

            if (evaluteHeightFromBiomes)
                heightMap = biomesGenerator.EvaluteHeightByBiomes(heightMap, globalBiomeMask);

            DisplayMap();
        }

        void GenerateGlobalMap()
        {
            heightMap = Noise.GenerateNoiseMap(
                mapWidth, mapHeight, settings.noiseScale,
                settings.octaves, settings.persistance,
                settings.lacunarity, settings.offset);
        }

        void DisplayMap()
        {
            // Generate color map
            Color[] colorMap;

            // Choose map display mode
            if (displayType == DisplayType.GameView)
            {
                if (FastBiomeColoring)
                    colorMap = ColorMapGenerator.ColorMapFromColorRegions(heightMap, globalBiomeMask, biomesGenerator.biomes);
                else
                    colorMap = ColorMapGenerator.ColorMapFromColorRegions(heightMap, biomesMasks, biomesGenerator.biomes);
            }
            else if (displayType == DisplayType.Biomes)
                colorMap = biomesGenerator.CreateColorMap(globalBiomeMask);
            else
                colorMap = ColorMapGenerator.ColorMapFromHeightMap(heightMap);

            ColorPack[,] convertedColorMap = ColorMapGenerator.ConvertColorMap(mapWidth, mapHeight, colorMap);
            for (int i = 0; i < SmoothIterations; i++)
                convertedColorMap = ColorMapGenerator.SmoothColorMap(convertedColorMap);

            mapDisplay.DrawMesh(
                MeshEditor.GenerateNewMesh(
                    heightMap, triangleRangeXScale, triangleRangeYScale, 
                    triangleSizeScale, meshHeightMultiplier, 
                    meshHeightCurve, convertedColorMap)
                );
        }

        public enum DisplayType
        {
            Noise,
            Biomes,
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

