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
    }
}
