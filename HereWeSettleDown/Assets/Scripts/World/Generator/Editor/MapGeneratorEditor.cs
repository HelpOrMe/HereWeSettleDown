using UnityEngine;
using UnityEditor;
using World.Generator.Chunks;

namespace World.Generator
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

            if (autoUpdate || GUILayout.Button("Generate map"))
            {
                foreach (Transform chunk in FindObjectOfType<ChunkGenerator>().transform)
                {
                    Destroy(chunk.gameObject);
                }
                mapGenerator.GenerateMap();
            }
        }
    }
}
