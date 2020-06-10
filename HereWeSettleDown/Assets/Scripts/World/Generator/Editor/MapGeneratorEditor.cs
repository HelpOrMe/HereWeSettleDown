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
                GeneratorPart.InvokePart<MoisturePart>();
                GeneratorPart.InvokePart<BiomesPart>();
                GeneratorPart.InvokePart<ColorsPart>();
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
