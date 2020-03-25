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
            if (GUILayout.Button("Recolor map"))
            {
                new BiomesPart().action();
                new ColorsPart().action();
                Map.WorldMesh.ConfrimChangeSplited();
            }
        }
    }
}
