using UnityEngine;
using XNode;

namespace World.Generator.Nodes.HeightMap.Functions
{
    public class Evalute : Node
    {
        [Input] public HeightMap heightMap;
        public AnimationCurve curve;
        [Output] public HeightMap outMap;

        public HeightMap GetOutMap()
        {
            HeightMap heightMap = GetInputValue<HeightMap>("heightMap");

            if (heightMap != null)
            {
                for (int x = 0; x < heightMap.width; x++)
                {
                    for (int y = 0; y < heightMap.height; y++)
                    {
                        heightMap[x, y] = curve.Evaluate(heightMap[x, y]);
                    }
                }
                return heightMap;
            }
            return null;
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMap")
                return GetOutMap();
            return null;
        }
    }
}
