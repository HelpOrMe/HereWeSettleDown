using UnityEngine;
using UnityEditor;

namespace Generator.Test._Editor
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MapGenerator mapGenerator = (MapGenerator)target;
            if (mapGenerator.autoUpdate)
            {
                mapGenerator.GenerateMap();
            }
            if (GUILayout.Button("Generate map"))
            {
                mapGenerator.GenerateMap();
            }
        }
    }
}
