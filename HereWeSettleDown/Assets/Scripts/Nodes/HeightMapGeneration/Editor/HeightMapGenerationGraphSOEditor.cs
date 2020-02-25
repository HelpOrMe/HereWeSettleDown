using UnityEditor;
using UnityEngine;

namespace Nodes.HeightMapGeneration
{
    [CustomEditor(typeof(HeightMapGenerationGraph))]
    public class HeightMapGenerationGraphSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var graph = (HeightMapGenerationGraph)target;
            if (GUILayout.Button("Clear prng"))
                graph.setPrng = null;
            if (GUILayout.Button("Set prng"))
                graph.setPrng = new System.Random(graph.editorSeed);
        }
    }
}
