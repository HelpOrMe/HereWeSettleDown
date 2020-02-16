using UnityEngine;


namespace Generator.Custom
{
    public class MapGenerator : MasterGenerator
    {
        public int seed;

        public int mapWidth;
        public int mapHeight;

        public GenerationSettings settings;

        public void SetMapGenerationValues()
        {
            SubGenerator.values.Clear();
            SubGenerator.values["mapWidth"] = mapWidth;
            SubGenerator.values["mapHeight"] = mapHeight;
            SubGenerator.values["mapGenerationSettings"] = settings;
        }

        public void GenerateMap()
        {
            Noise.SetupPRNG(seed);
            SetMapGenerationValues();
            RegistrateGenerators();
            StartGenerate();
        }

        public override void OnGenerationEnd()
        {
            Debug.Log("Yeeeey!");
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
