using UnityEngine;
using XNode;

namespace Nodes.HeightMapGeneration.Other
{
    public class MapRequester : Node
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public HeightMap heightMap;

        public HeightMap GetHeightMap()
        {
            return GetInputValue<HeightMap>("heightMap");
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}
