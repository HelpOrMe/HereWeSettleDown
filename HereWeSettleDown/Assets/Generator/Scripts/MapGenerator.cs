using UnityEngine;

namespace Generator
{
    public class MapGenerator : MonoBehaviour
    {
        public int seed;

        public int mapWidth;
        public int mapHeight;
        
        public GenerationSettings settings;
        public float meshHeightMultiplier;

        public bool evoluteBiomesHeight;

        public MapDisplay mapDisplay;
        public DisplayType displayType;
        public BiomesMapGenerator biomesGenerator;

        [HideInInspector] static public float[,] map;
        [HideInInspector] static public int[,] biomesMask;

        public void GenerateMap()
        {
            CorrectValuesInGenerators();
            GenerateGlobalMap();

            // Generate biomes mask
            BiomePosition[] biomesPositions = biomesGenerator.RandomizeBiomePositions();
            biomesMask = biomesGenerator.GenerateBiomeMask(biomesPositions);

            // Add biome noise
            if (evoluteBiomesHeight)
                map = biomesGenerator.EvoluteBiomesHeight(map, biomesMask);

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
            map = Noise.GenerateNoiseMap(
                seed, mapWidth, mapHeight,
                settings.heightCurve, settings.noiseScale,
                settings.octaves, settings.persistance,
                settings.lacunarity, settings.offset);
        }

        void DisplayMap()
        {
            Texture2D texture;

            if (displayType == DisplayType.Biomes)
                texture = TextureGenerator.TextureFromColourMap(biomesGenerator.CreateColorMap(biomesMask), mapWidth, mapHeight);
            else
                texture = TextureGenerator.TextureFromHeightMap(map);

            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(map, meshHeightMultiplier), texture);
        }

        public enum DisplayType
        {
            Noise,
            Biomes
        }
    }

    [System.Serializable]
    public struct GenerationSettings
    {
        public float noiseScale;
        public AnimationCurve heightCurve;
        public int octaves;
        [Range(0f, 1f)]
        public float persistance;
        public float lacunarity;
        public Vector2 offset;
    }
}

