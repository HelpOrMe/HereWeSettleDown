using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class SettingsObject : ScriptableObject
    {
        private static Dictionary<Type, object> allSettingsObjects = new Dictionary<Type, object>();

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
    }
}
