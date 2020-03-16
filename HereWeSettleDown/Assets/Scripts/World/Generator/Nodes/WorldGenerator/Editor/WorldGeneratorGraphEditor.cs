using System;
using XNodeEditor;

namespace World.Generator.Nodes.WorldGenerator
{
    [CustomNodeGraphEditor(typeof(WorldGeneratorGraph))]
    public class WorldGeneratorGraphEditor : NodeGraphEditor
    {
        public override string GetNodeMenuName(Type type)
        {
            return base.GetNodeMenuName(type).Replace("World/Generator/Nodes/", "");
        }
    }
}
