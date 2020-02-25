using XNode;

namespace Nodes.HeightMapGeneration.Maps
{
    public class FlatMap : Node
    {
        [Output] public HeightMap outMap;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMap")
            {
                var ghp = (HeightMapGenerationGraph)graph;
                return new HeightMap(new float[ghp.mapWidth, ghp.mapHeight]);
            }
            return null;
        }
    }
}
