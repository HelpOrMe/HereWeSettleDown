using UnityEngine;
using World.MeshSystem;

namespace Generator.Custom
{
    public class MapGenerator : MasterGenerator
    {
        public int seed;

        public int mapWidth;
        public int mapHeight;

        public GenerationSettings settings;

        public void GenerateMap()
        {
            Noise.SetupPRNG(seed);
            SetMapGenerationValues();
            RegistrateGenerators();
            StartGenerate();
        }

        public void SetMapGenerationValues()
        {
            SubGenerator.values.Clear();
            SubGenerator.values["mapWidth"] = mapWidth;
            SubGenerator.values["mapHeight"] = mapHeight;
            SubGenerator.values["mapGenerationSettings"] = settings;
        }

        public override void OnGenerationEnd()
        {
            print("Yeeey!");
        }
    }

    [System.Serializable]
    public struct GenerationSettings
    {
        public AnimationCurve heightCurve;
        public float noiseScale;
        public int octaves;
        [Range(0f, 1f)]
        public float persistance;
        public float lacunarity;
        public Vector2 offset;
    }
}
