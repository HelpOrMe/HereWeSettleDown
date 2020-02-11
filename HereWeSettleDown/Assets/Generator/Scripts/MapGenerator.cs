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

        [HideInInspector] static public float[,] map;
        [HideInInspector] static public int[,] biomesMask;

        public void GenerateMap()
        {
            CorrectValuesInGenerators();
            GenerateGlobalMap();
            biomesMask = biomesGenerator.GenerateBiomeMask();
            if (EvoluteHeight)
                map = biomesGenerator.EvoluteHeightByBiomes(map, biomesMask);

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
                seed, mapWidth, mapHeight, settings.noiseScale,
                settings.octaves, settings.persistance,
                settings.lacunarity, settings.offset);
        }

        void DisplayMap()
        {
            Texture2D texture;

            if (displayType == DisplayType.GameView)
                texture = TextureGenerator.TextureFromColorRegions(map, biomesMask, biomesGenerator.biomes);
            else if (displayType == DisplayType.Biomes)
                texture = TextureGenerator.TextureFromColorMap(biomesGenerator.CreateColorMap(biomesMask), mapWidth, mapHeight);
            else
                texture = TextureGenerator.TextureFromHeightMap(map);

            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(map, meshHeightMultiplier, meshHeightCurve), texture);
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

