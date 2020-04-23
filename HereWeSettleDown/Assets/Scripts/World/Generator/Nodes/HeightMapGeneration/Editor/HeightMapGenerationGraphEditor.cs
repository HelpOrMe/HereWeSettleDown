using System;
using XNodeEditor;
using UnityEngine;

namespace World.Generator.Nodes.HeightMap
{
    [CustomNodeGraphEditor(typeof(HeightMapGenerationGraph))]
    public class HeightMapGenerationGraphEditor : NodeGraphEditor
    {
        public override string GetNodeMenuName(Type type)
        {
            Debug.Log(base.GetNodeMenuName(type));
            return base.GetNodeMenuName(type).Replace("World/Generator/Nodes/", "");
        }
    }
}
