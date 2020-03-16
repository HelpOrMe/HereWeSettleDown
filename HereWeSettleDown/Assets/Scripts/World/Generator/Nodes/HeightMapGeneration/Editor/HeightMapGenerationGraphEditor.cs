using System;
using XNodeEditor;

namespace World.Generator.Nodes.HeightMap
{
    [CustomNodeGraphEditor(typeof(HeightMapGenerationGraph))]
    public class HeightMapGenerationGraphEditor : NodeGraphEditor
    {
        public override string GetNodeMenuName(Type type)
        {
            return base.GetNodeMenuName(type).Replace("World/Generator/Nodes/", "");
        }
    }
}
