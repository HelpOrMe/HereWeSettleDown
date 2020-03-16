using UnityEngine;
using XNode;

namespace World.Generator.Nodes.HeightMap.Functions
{
    public class Clamp : Node
    {
        [Input] public HeightMap heightMap;
        public float minValue = 0;
        public float maxValue = 1;
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
                        heightMap[x, y] = Mathf.Clamp(heightMap[x, y], minValue, maxValue);
                    }
                }
            }

            return heightMap;
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMap")
                return GetOutMap();
            return null;
        }
    }
}
