using System;
using XNodeEditor;

namespace World.Generator.Nodes
{
    [CustomNodeGraphEditor(typeof(WorldGeneratorGraph))]
    public class WorldGeneratorGraphEditor : NodeGraphEditor
    {
        public override string GetNodeMenuName(Type type)
        {
            return base.GetNodeMenuName(type).Replace("Generator/Nodes/", "");
        }
    }
}
