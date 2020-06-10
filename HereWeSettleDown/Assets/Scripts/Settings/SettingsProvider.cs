using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.UIElements;

namespace Settings
{
    public class GameSettingsProvider : SettingsProvider
    {
        public static readonly string dataPath = /*Application.dataPath + */"Assets/Settings";
        private static readonly Dictionary<Type, SettingsObject> m_settingsInstance = new Dictionary<Type, SettingsObject>();

        public GameSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project) : base(path, scopes) { }

        public static T GetSettings<T>() where T : SettingsObject
        {
            return (T)GetSettings(typeof(T));
        }

        public static object GetSettings(Type t)
        {
            if (t != null && t.IsSubclassOf(typeof(SettingsObject)))
            {
                if (!m_settingsInstance.ContainsKey(t) || m_settingsInstance[t] == null)
                {
                    m_settingsInstance[t] = (SettingsObject)LoadOrCreateSettings(t);
                }
                return m_settingsInstance[t];
            }
            return null;
        }

        public static object LoadOrCreateSettings(Type t)
        {
            if (t != null && t.IsSubclassOf(typeof(SettingsObject)))
            {
                object obj = AssetDatabase.LoadAssetAtPath($"{dataPath}/{t.Name}.asset", t);
                if (obj == null)
                {
                    obj = ScriptableObject.CreateInstance(t);
                    AssetDatabase.CreateAsset((SettingsObject)obj, $"{dataPath}/{t.Name}.asset");
                }
                return obj;
            }
            return null;
        }

        public static void LoadAllSettings()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(SettingsObject)))
                {
                    GetSettings(t);
                }
            }
        }

        public static void SaveSettings()
        {
            AssetDatabase.SaveAssets();
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            LoadAllSettings();
        }

        public override void OnGUI(string searchContext)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label) 
            { 
                alignment = TextAnchor.MiddleCenter, 
                fontStyle = FontStyle.Bold 
            };

            foreach (var pair in m_settingsInstance)
            {
                if (GUILayout.Button(pair.Key.Name, style))
                {
                    Selection.activeObject = pair.Value;
                    EditorUtility.FocusProjectWindow();
                }
                SerializedObject sObj = new SerializedObject(pair.Value);
                foreach (FieldInfo field in pair.Key.GetFields())
                {
                    EditorGUILayout.PropertyField(sObj.FindProperty(field.Name), true);
                }
                EditorGUILayout.Space();
                sObj.ApplyModifiedProperties();
                sObj.Dispose();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new GameSettingsProvider("Project/Settings", SettingsScope.Project);
        }
    }
}
