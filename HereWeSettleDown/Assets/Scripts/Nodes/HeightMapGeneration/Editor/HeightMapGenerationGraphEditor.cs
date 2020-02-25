using System;
using XNodeEditor;

namespace Nodes.HeightMapGeneration
{
    [CustomNodeGraphEditor(typeof(HeightMapGenerationGraph))]
    public class HeightMapGenerationGraphEditor : NodeGraphEditor
    {
        public override string GetNodeMenuName(Type type)
        {
            return base.GetNodeMenuName(type).Replace("Nodes/","");
        }
    }
}
