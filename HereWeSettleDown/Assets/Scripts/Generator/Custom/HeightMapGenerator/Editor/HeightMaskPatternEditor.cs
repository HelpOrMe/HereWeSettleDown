using UnityEditor;

namespace Generator.Custom
{
    [CustomEditor(typeof(HeightMaskPattern))]
    public class HeightMaskPatternEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            HeightMaskPattern pattern = (HeightMaskPattern)target;

            Field("maskType");
            switch (pattern.maskType)
            {
                case (HeightMaskPatternType.Absolute):
                    Field("absoluteSettings", true); break;
                case (HeightMaskPatternType.Noise):
                    Field("noiseSettings", true); break;
                case (HeightMaskPatternType.Border):
                    Field("borderSettings", true); break;
                case (HeightMaskPatternType.Picks):
                    Field("picksSettings", true); break;
            }

            Field("appendMasksPattern", true);
            Field("subtractMasksPattern", true);

            serializedObject.ApplyModifiedProperties();
        }

        public void Field(string name, bool child = false)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(name), child);
        }
    }
}
