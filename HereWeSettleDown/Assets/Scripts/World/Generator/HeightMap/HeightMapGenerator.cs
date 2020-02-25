using Nodes.HeightMapGeneration;

namespace World.Generator.HeightMap
{
    [CustomGenerator(true, "mapWidth", "mapHeight")]
    public class HeightMapGenerator : SubGenerator
    {
        public HeightMapGenerationGraph graph;

        public override void OnGenerate()
        {
            graph.mapWidth = GetValue<int>("mapWidth");
            graph.mapHeight = GetValue<int>("mapHeight");
            graph.setPrng = ownPrng;
            values["heightMap"] = graph.requester.GetHeightMap().map;
            GenerationCompleted();
        }
    }
}
