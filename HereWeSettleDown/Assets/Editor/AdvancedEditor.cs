using UnityEditor;

namespace _Editor
{
    public class AdvancedEditor : Editor
    {
        public void Field(string name, bool child = false)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(name), child);
        }
    }
}

