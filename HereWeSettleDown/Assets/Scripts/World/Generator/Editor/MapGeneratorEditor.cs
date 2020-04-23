using UnityEngine;
using UnityEditor;

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
                mapGenerator.GenerateMap();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset color"))
            {
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
        }
    }
}
