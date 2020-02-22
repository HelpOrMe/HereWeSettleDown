using UnityEditor;
using World.Generator.HeightMap;

namespace _Editor.Inspector
{
    [CustomEditor(typeof(HeightMaskPattern))]
    public class HeightMaskPatternEditor : AdvancedEditor
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

            Field("appendMasksPatterns", true);
            Field("subtractMasksPatterns", true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
