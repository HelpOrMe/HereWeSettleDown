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

            if (GUILayout.Button("Generate map"))
            {
                /*foreach (Transform chunk in FindObjectOfType<ChunkGenerator>().transform)
                {
                    Destroy(chunk.gameObject);
                }*/
                mapGenerator.GenerateMap();
            }
        }
    }
}
