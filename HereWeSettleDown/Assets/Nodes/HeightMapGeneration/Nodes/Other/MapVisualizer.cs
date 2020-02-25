using UnityEngine;
using XNode;

namespace Nodes.HeightMapGeneration.Other
{
    public class MapVisualizer : Node
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public HeightMap heightMap;
        [HideInInspector] public Texture2D texture;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}
