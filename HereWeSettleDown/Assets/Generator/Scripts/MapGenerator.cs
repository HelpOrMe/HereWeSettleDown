using UnityEngine;

namespace Generator
{
    public class MapGenerator : MonoBehaviour
    {
        public int mapWidth;
        public int mapHeight;
        public int seed;

        public float meshHeightMultiplier;

        public MapDisplay mapDisplay;
        public DisplayType displayType;
        public BiomesMapGenerator biomesGenerator;

        [HideInInspector] static public float[,] map;
        [HideInInspector] static public int[,] biomesMask;

        public void GenerateMap()
        {
            CorrectValuesInGenerators();

            // Generate biomes mask
            BiomePosition[] biomes = biomesGenerator.RandomizeBiomePositions();
            biomesMask = biomesGenerator.GenerateBiomeMask(biomes);

            // Generate biome map
            map = biomesGenerator.GenerateBiomeMap(biomesMask);

            DisplayMap();
        }

        void CorrectValuesInGenerators()
        {
            biomesGenerator.mapWidth = mapWidth;
            biomesGenerator.mapHeight = mapHeight;
            biomesGenerator.seed = seed;
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
}

