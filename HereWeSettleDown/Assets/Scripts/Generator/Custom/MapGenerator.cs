using System.Collections;
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
            StartCoroutine(Hehe());
        }

        public IEnumerator Hehe()
        {
            Camera cam = Camera.main;
            while (true)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    for (int i = 0; i < 2; i++)
                        MeshMap.SetColor((int)hit.point.x / 10, (int)hit.point.z / 10, i, Color.white);
                    MeshMap.ConfirmChanges();
                }
                
                yield return new WaitForEndOfFrame();
            }
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
