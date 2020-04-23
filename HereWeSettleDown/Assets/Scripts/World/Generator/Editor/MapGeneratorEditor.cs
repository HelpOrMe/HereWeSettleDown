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

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Debug moisture"))
            {
                mapGenerator.Debug_DrawMoisture();
            }
            if (GUILayout.Button("Debug height"))
            {
                mapGenerator.Debug_DrawHeight();
            }
            GUILayout.EndHorizontal();
        }
    }
}
