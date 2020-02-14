using UnityEngine;

namespace Generator
{
    public class MapGenerator : MonoBehaviour
    {
        public int seed;

        public int chunkWidth;
        public int chunkHeight;

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

        public GameObject chunkTerrain;
        public BiomesMapGenerator biomesGenerator;

        public static float[,] heightMap;
        public static BiomeMask[] biomesMasks;
        public static int[,] globalBiomeMask;

        public static Chunk[,] chunkMap;

        public void GenerateMap()
        {
            Noise.SetupPRNG(seed);

            GenerateGlobalMap();
            biomesMasks = biomesGenerator.GenerateBiomesMask(mapWidth, mapHeight);
            globalBiomeMask = biomesGenerator.СombineMasks(mapWidth, mapHeight, biomesMasks);
            if (evaluteHeightFromBiomes)
                heightMap = biomesGenerator.EvaluteHeightByBiomes(heightMap, globalBiomeMask);

            GenerateChunks();
        }

        void GenerateGlobalMap()
        {
            heightMap = Noise.GenerateNoiseMap(
                mapWidth, mapHeight, settings.noiseScale,
                settings.octaves, settings.persistance,
                settings.lacunarity, settings.offset);
        }

        void GenerateChunks()
        {
            // Generate meshes
            ColorPack[,] colorMap = GenerateColorMap();
            MeshData[,] chunkMeshesMap = MeshEditor.GenerateChunkMeshes(
                chunkWidth, chunkHeight, mapWidth / chunkWidth, 
                mapHeight / chunkHeight, heightMap, 
                triangleRangeXScale, triangleRangeYScale, 
                triangleSizeScale, meshHeightMultiplier, 
                meshHeightCurve, colorMap);

            // Setup chunks
            int width = mapWidth / chunkWidth;
            int height = mapHeight / chunkHeight;
            chunkMap = new Chunk[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    chunkMap[x, y] = new Chunk(chunkTerrain, new Vector2Int(x, y), new Vector2Int(chunkWidth, chunkHeight), chunkMeshesMap[x, y]);
                }
            }
        }

        ColorPack[,] GenerateColorMap()
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

            return convertedColorMap;
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

