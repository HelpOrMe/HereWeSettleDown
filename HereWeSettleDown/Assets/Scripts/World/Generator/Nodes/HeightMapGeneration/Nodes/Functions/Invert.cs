using UnityEngine;
using XNode;

namespace World.Generator.Nodes.HeightMap.Functions
{
    public class Invert : Node
    {
        [Input] public HeightMap heightMap;
        [Output] public HeightMap outMap;

        public HeightMap GetOutMap()
        {
            HeightMap heightMap = GetInputValue<HeightMap>("heightMap");

            if (heightMap != null)
            {
                float maxValue = float.MinValue;

                for (int x = 0; x < heightMap.width; x++)
                {
                    for (int y = 0; y < heightMap.height; y++)
                    {
                        if (heightMap[x, y] > maxValue)
                        {
                            maxValue = heightMap[x, y];
                        }
                    }
                }

                if (maxValue < 1)
                {
                    maxValue = 1;
                }

                for (int x = 0; x < heightMap.width; x++)
                {
                    for (int y = 0; y < heightMap.height; y++)
                    {
                        heightMap[x, y] = maxValue - heightMap[x, y];
                    }
                }

                return heightMap;
            }
            return null;
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
