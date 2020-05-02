using System.Reflection;
using System.Collections.Generic;   
using UnityEngine;
using UnityEditor;
using World.Generator._Debug;

namespace World.Generator
{
    [CustomEditor(typeof(MapGenerator), true)]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MapGenerator mapGenerator = (MapGenerator)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("Generate map"))
            {
                mapGenerator.GenerateMap();
            }

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset color"))
            {
                new MoisturePart().action();
                new BiomesPart().action();
                new ColorsPart().action();
                Map.WorldMesh.ConfrimChangeSplitted();
            }
            if (GUILayout.Button("Reset height"))
            {
                new HeightPart().action();
                Map.WorldMesh.ConfrimChangeSplitted();
            }
            GUILayout.EndHorizontal();

            // Ignore equal, toString, etc. methods
            foreach (var method in typeof(GeneratorDebuger).GetMethods())
            {
                if (method.Name == "Equals")
                    break;

                if (GUILayout.Button(method.Name))
                {
                    method.Invoke(null, null);
                }
            }
        }
    }
}
