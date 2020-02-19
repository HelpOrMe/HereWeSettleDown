
namespace World.Generator.Global
{
    public class MapGenerator : MasterGenerator
    {
        public int mapWidth;
        public int mapHeight;

        public void GenerateMap()
        {
            SetMapGenerationValues();
            RegistrateGenerators();
            StartGenerate();
        }

        private void SetMapGenerationValues()
        {
            SubGenerator.values.Clear();
            SubGenerator.values["mapWidth"] = mapWidth;
            SubGenerator.values["mapHeight"] = mapHeight;
        }
    }

}
