using World.Generator.Helper;
using XNode;

namespace World.Generator.Nodes.HeightMap.Maps
{
    public class FalloffMap : Node
    {
        [Output] public HeightMap outMap;

        public HeightMap GetOutMap()
        {
            HeightMapGenerationGraph ghp = (HeightMapGenerationGraph)graph;
            float[,] outMap = Noise.GenerateFalloffMap(ghp.mapWidth, ghp.mapHeight);
            return new HeightMap(outMap);
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMap")
            {
                return GetOutMap();
            }

            return null;
        }
    }
}
