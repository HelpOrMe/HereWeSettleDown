using UnityEngine;
using XNode;

namespace World.Generator.Nodes.HeightMap.Functions
{
    public class Lerp : Node
    {
        [Input] public HeightMap firstMap;
        [Input] public HeightMap secondMap;
        [Range(0, 1)] public float time = 0.5f;
        [Output] public HeightMap outMap;

        public HeightMap GetOutMap()
        {
            HeightMap fMap = GetInputValue<HeightMap>("firstMap");
            HeightMap sMap = GetInputValue<HeightMap>("secondMap");

            for (int x = 0; x < fMap.width; x++)
            {
                for (int y = 0; y < fMap.height; y++)
                {
                    fMap[x, y] = Mathf.Lerp(fMap[x, y], sMap[x, y], time);
                }
            }

            return fMap;
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMap")
                return GetOutMap();
            return null;
        }
    }
}
