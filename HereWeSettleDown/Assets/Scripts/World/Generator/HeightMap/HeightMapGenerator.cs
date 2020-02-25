using Nodes.HeightMapGeneration;
using Nodes.HeightMapGeneration.Other;

namespace World.Generator.HeightMap
{
    [CustomGenerator(true, "mapWidth", "mapHeight")]
    public class HeightMapGenerator : SubGenerator
    {
        public HeightMapGenerationGraph graph;
        public MapRequester requester;

        public override void OnGenerate()
        {
            graph.mapWidth = GetValue<int>("mapWidth");
            graph.mapHeight = GetValue<int>("mapHeight");
            values["heightMap"] = requester.GetHeightMap().map;
            GenerationCompleted();
        }
    }
}
