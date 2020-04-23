using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using XNodeEditor;

namespace World.Generator.Nodes.WorldGenerator.Parts
{
    [CustomNodeEditor(typeof(Part))]
    public class GeneratorActionEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            var node = (Part)target;
            var graph = (WorldGeneratorGraph)node.graph;
            Type[] types = graph.GetPartTypes();

            if (types.Length > 0)
            {
                node.selectedPart = EditorGUILayout.Popup("Part type", node.selectedPart, GetTypesInString(types));
                if (node.selectedPart >= types.Length)
                    node.selectedPart = types.Length - 1;

                DrawTypeFields(types[node.selectedPart]);
                base.OnBodyGUI();
            }
            else EditorGUILayout.LabelField("No parts found");
        }

        public string[] GetTypesInString(Type[] types)
        {
            string[] strTypes = new string[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                strTypes[i] = types[i].Name;
            }
            return strTypes;
        }

        public void DrawTypeFields(Type type)
        {
            var node = (Part)target;
            object obj = node.GetPartObject();

            foreach (FieldInfo field in type.GetFields())
            {
                if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
                {
                    node.partObjectFields[field.Name] = EditorGUILayout.ObjectField(field.Name, (UnityEngine.Object)field.GetValue(obj), field.FieldType, true);
                }
            }
        }
    }
}
