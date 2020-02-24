using UnityEngine;
using XNode;
using World.Generator.Helper;

namespace Nodes.HeightMapGeneration.Maps
{
    public class FalloffMap : Node
    {
        public AnimationCurve heightCurve;
        [Output] public HeightMap outMap;

        public HeightMap GetOutMap()
        {
            var ghp = (HeightMapGenerationGraph)graph;
            float[,] outMap = Noise.GenerateFalloffMap(ghp.mapWidth, ghp.mapHeight);
            for (int x = 0; x < ghp.mapWidth; x++)
            {
                for (int y = 0; y < ghp.mapHeight; y++)
                {
                    outMap[x, y] = heightCurve.Evaluate(outMap[x, y]);
                }
            }
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
