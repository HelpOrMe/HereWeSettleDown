using UnityEngine;
using XNodeEditor;

namespace World.Generator.Nodes.WorldGenerator.Parts
{
    [CustomNodeEditor(typeof(PartCombiner))]
    public class PartCombinerEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            var gen = (PartCombiner)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add part layer"))
                gen.AddPartLayer();
            if (GUILayout.Button("Remove part layer"))
                gen.RemoveLastPartLayer();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Remove empty part layers"))
                gen.RemoveEmptyPartLayers();
        }
    }
}
