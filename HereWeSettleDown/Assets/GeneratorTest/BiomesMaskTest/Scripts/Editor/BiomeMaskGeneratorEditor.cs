using UnityEngine;
using UnityEditor;

namespace Generator.Test.BiomeMask._Editor
{
    [CustomEditor(typeof(BiomesMaskGenerator))]
    public class BiomeMaskGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BiomesMaskGenerator maskGenerator = (BiomesMaskGenerator)target;
            if (maskGenerator.autoUpdate)
            {
                maskGenerator.GenerateMap();
            }
            if (GUILayout.Button("Generate map"))
            {
                maskGenerator.GenerateMap();
            }
        }
    }
}
