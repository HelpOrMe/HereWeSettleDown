using UnityEditor;
using World.Generator.Biomes;

namespace _Editor.Inspector
{
    [CustomEditor(typeof(BiomePattern))]
    public class BiomePatternEditor : AdvancedEditor
    {
        public override void OnInspectorGUI()
        {
            BiomePattern biome = (BiomePattern)target;

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
