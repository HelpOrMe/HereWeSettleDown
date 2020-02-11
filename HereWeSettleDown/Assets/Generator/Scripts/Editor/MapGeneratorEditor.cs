using UnityEngine;
using UnityEditor;

namespace Generator._Editor
{
    [CustomEditor(typeof(MapGenerator), true)]
    public class MapGeneratorEditor : Editor
    {
        static bool autoUpdate;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MapGenerator mapGenerator = (MapGenerator)target;

            autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);

            if (autoUpdate)
                mapGenerator.GenerateMap();

            if (GUILayout.Button("Generate map"))
                mapGenerator.GenerateMap();
        }
    }
}
