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
            if (!gph.requesters.Contains(this))
            {
                gph.requesters.Add(this);
            }
        }

        private void OnDestroy()
        {
            HeightMapGenerationGraph gph = (HeightMapGenerationGraph)graph;
            gph.requesters.Remove(this);
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
