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

            if (autoUpdate || GUILayout.Button("Generate map"))
            {
                if (MapGenerator.chunkMap != null)
                {
                    for (int x = 0; x < MapGenerator.chunkMap.GetLength(0); x++)
                    {
                        for (int y = 0; y < MapGenerator.chunkMap.GetLength(1); y++)
                        {
                            if (MapGenerator.chunkMap[x, y].chunkObject)
                                Destroy(MapGenerator.chunkMap[x, y].chunkObject);
                        }
                    }
                }
                
                mapGenerator.GenerateMap();
            }
        }
    }
}
