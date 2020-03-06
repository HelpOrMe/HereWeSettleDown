using UnityEngine;
using XNode;
using World.Generator.Helper;

namespace Nodes.HeightMapGeneration.Maps
{
    public class FalloffMap : Node
    {
        [Output] public HeightMap outMap;

        public HeightMap GetOutMap()
        {
            var ghp = (HeightMapGenerationGraph)graph;
            float[,] outMap = Noise.GenerateFalloffMap(ghp.mapWidth, ghp.mapHeight);
            return new HeightMap(outMap);
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMap")
                return GetOutMap();
            return null;
        }
    }
}
