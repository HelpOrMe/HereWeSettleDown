using UnityEngine;
using XNode;

namespace World.Generator.Nodes.HeightMap.Functions
{
    public class Math : Node
    {
        [Input] public HeightMap firstMap;
        [Input] public HeightMap secondMap;
        public MathFunc function;
        [Output] public HeightMap outMap;
        public enum MathFunc { Add, Subtract, Multiply, Divide };

        public HeightMap GetOutMap()
        {
            HeightMap fMap = GetInputValue<HeightMap>("firstMap");
            HeightMap sMap = GetInputValue<HeightMap>("secondMap");

            if (fMap != null && sMap != null)
            {
                for (int x = 0; x < fMap.width; x++)
                {
                    for (int y = 0; y < fMap.height; y++)
                    {
                        float value;
                        switch (function)
                        {
                            case MathFunc.Add:
                            default:
                                value = fMap[x, y] + sMap[x, y]; break;
                            case MathFunc.Subtract:
                                value = fMap[x, y] - sMap[x, y]; break;
                            case MathFunc.Multiply:
                                value = fMap[x, y] * sMap[x, y]; break;
                            case MathFunc.Divide:
                                value = fMap[x, y] / Mathf.Clamp(sMap[x, y], 0.0001f, Mathf.Infinity); break;
                        }
                        fMap[x, y] = value;
                    }
                }
                return fMap;
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
