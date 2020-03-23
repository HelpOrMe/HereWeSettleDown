using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class SettingsObject : ScriptableObject
    {
        [SerializeField] private static AllSettingsObjectsDictionary allSettingsObjects = new AllSettingsObjectsDictionary();

        public static T GetObject<T>()
        {
            if (allSettingsObjects.ContainsKey(typeof(T)))
                return (T)allSettingsObjects[typeof(T)];
            return default;
        }

        private void OnEnable()
        {
            if (allSettingsObjects.ContainsKey(GetType()))
            {
                DestroyImmediate((UnityEngine.Object)allSettingsObjects[GetType()]);
                allSettingsObjects.Remove(GetType());
            }
            allSettingsObjects.Add(GetType(), this);
        }

        [Serializable]
        public class AllSettingsObjectsDictionary : Dictionary<Type, object>, ISerializationCallbackReceiver
        {
            [SerializeField] private List<string> keys = new List<string>();
            [SerializeField] private List<SettingsObject> values = new List<SettingsObject>();

            public void OnBeforeSerialize()
            {
                keys.Clear();
                values.Clear();
                foreach (KeyValuePair<Type, object> pair in this)
                {
                    keys.Add(pair.Key.ToString());
                    values.Add((SettingsObject)pair.Value);
                }
            }

            public void OnAfterDeserialize()
            {
                Clear();
                for (int i = 0; i < keys.Count; i++)
                {
                    Add(Type.GetType($"Settings.{keys[i]}, Assembly-CSharp", false), values[i]);
                }
            }
        }
    }
}
