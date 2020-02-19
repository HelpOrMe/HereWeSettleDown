using UnityEditor;

namespace World.Generator.Biomes._Editor
{
    [CustomEditor(typeof(Biome))]
    public class BiomeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Biome biome = (Biome)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mapColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("index"));
            EditorGUILayout.Space();

            if (!biome.spawnOnAllBiomes)
                EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnOnAllBiomes"));
            if (!biome.spawnOnAllBiomes)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnOnBiomes"), true);
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("heightMaskPatterns"), true);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isEvaluteHeight"));
            if (biome.isEvaluteHeight)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("evaluteHeight"));
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideColors"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("colorRegions"), true);

            serializedObject.ApplyModifiedProperties();
        }

        public void Field(string name, bool child = false)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(name), child);
        }
    }
}
