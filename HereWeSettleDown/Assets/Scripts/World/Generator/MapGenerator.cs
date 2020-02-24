using UnityEngine;
using World.Map;

namespace World.Generator
{
    public class MapGenerator : MasterGenerator
    {
        public int mapWidth;
        public int mapHeight;

        private void SetMapGenerationValues()
        {
            SubGenerator.values.Clear();
            SubGenerator.values["mapWidth"] = mapWidth;
            SubGenerator.values["mapHeight"] = mapHeight;
        }

        public void GenerateMap()
        {
            SetMapGenerationValues();
            RegistrateGenerators();
            StartGenerate();
        }

        public override void OnGenerationEnd()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        WorldMeshMap.colorMap[x, y, i] = Color.red;
                    }
                }
            }
            WorldMeshMap.ConfirmChanges();
        }
    }
}
