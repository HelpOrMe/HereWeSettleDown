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
        public bool EvoluteHeightFromBiomes;

        public bool SmoothColorMap;

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
            if (EvoluteHeightFromBiomes)
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
            else
                colorMap = ColorMapGenerator.ColorMapFromHeightMap2(heightMap);

            //MeshData meshData = MeshGenerator.GenerateTerrainMesh(seed, heightMap, triangleSizeScale, meshHeightMultiplier, meshHeightCurve);

            //meshData.SetColorMap(colorMap);

            ColorPack[,] convertedColorMap = ColorMapGenerator.ConvertColorMap(mapWidth, mapHeight, colorMap);
            if (SmoothColorMap)
                convertedColorMap = ColorMapGenerator.SmoothColorMap(convertedColorMap);
            mapDisplay.DrawMesh(MeshEditor.GenerateNewMesh(seed, heightMap, triangleRangeXScale, triangleRangeYScale, triangleSizeScale, meshHeightMultiplier, meshHeightCurve, convertedColorMap));
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

