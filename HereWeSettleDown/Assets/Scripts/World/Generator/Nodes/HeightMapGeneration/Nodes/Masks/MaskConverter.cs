using UnityEngine;
using XNode;

namespace World.Generator.Nodes.HeightMap.Masks
{
    public class MaskConverter : Node
    {
        [Input] public HeightMap heightMap;
        public float minHeight = 0;
        public float maxHeight = 1;
        [Output] public HeightMap outMask;

        public HeightMap GetOutMask()
        {
            HeightMap heightMap = GetInputValue<HeightMap>("heightMap");
            if (heightMap != null)
            {
                float[,] mask = new float[heightMap.width, heightMap.height];
                for (int x = 0; x < heightMap.width; x++)
                {
                    for (int y = 0; y < heightMap.height; y++)
                    {
                        mask[x, y] = (minHeight < heightMap[x, y] && maxHeight > heightMap[x, y]) ? 1 : 0;
                    }
                }

                return new HeightMap(mask);
            }
            return null;
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMask")
                return GetOutMask();
            return null;
        }
    }
}
