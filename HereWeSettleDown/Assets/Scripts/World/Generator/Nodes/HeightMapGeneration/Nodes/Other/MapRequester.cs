using UnityEngine;
using XNode;

namespace World.Generator.Nodes.HeightMap.Other
{
    public class MapRequester : Node
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public HeightMap heightMap;

        protected override void Init()
        {
            HeightMapGenerationGraph gph = (HeightMapGenerationGraph)graph;
            gph.requester = this;
        }

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
