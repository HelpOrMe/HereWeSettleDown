using UnityEditor;
using _Editor;

namespace World.Generator.Biomes._Editor
{
    [CustomEditor(typeof(Biome))]
    public class BiomeEditor : AdvancedEditor
    {
        public override void OnInspectorGUI()
        {
            Biome biome = (Biome)target;

            Field("mapColor");
            EditorGUILayout.Space();

            if (!biome.SpawnOnAllBiomes)
                EditorGUILayout.Space();

            Field("SpawnOnAllBiomes");
            if (!biome.SpawnOnAllBiomes)
            {
                Field("spawnOnBiomes", true);
                EditorGUILayout.Space();
            }

            Field("UseDefaultHeightMap");
            if (!biome.UseDefaultHeightMap)
            {
                Field("noiseSettings", true);
            }
            else
            {
                Field("worldHeightCurve");
            }
            Field("heightMaskPattern");
            Field("power");
            EditorGUILayout.Space();

            Field("OverrideColors");
            Field("colorRegions", true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
