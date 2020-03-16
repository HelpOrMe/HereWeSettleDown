using UnityEngine;
using XNodeEditor;

namespace World.Generator.Nodes
{
    [CustomNodeEditor(typeof(Invoker))]
    public class InvokerEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            var invoker = (Invoker)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add action layer"))
                invoker.AddActionLayer();
            if (GUILayout.Button("Remove action layer"))
                invoker.RemoveLastActionLayer();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Remove empty action layers"))
                invoker.RemoveEmptyActionLayers();
        }
    }
}
