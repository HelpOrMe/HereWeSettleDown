using UnityEditor;

namespace Generator.Custom._Editor
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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("absolute"));
            if (!biome.absolute)
            {
                if (!biome.spawnOnAllBiomes)
                    EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnOnAllBiomes"));
                if (!biome.spawnOnAllBiomes)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnOnBiomes"), true);
                    EditorGUILayout.Space();
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("anySpawnHeight"));
                if (!biome.anySpawnHeight)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("minSpawnHeight"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSpawnHeight"));
                    EditorGUILayout.Space();
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("minMaskHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxMaskHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("copyBiomeMask"));
                if (biome.copyBiomeMask)
                {
                    EditorGUILayout.HelpBox("Don't loop biome masks!", MessageType.Info);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("biomeMaskParent"));
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maskSettings"), true);
                }
            }
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isEvaluteHeight"));
            if (biome.isEvaluteHeight)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("evaluteHeight"));
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideColors"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("colorRegions"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
