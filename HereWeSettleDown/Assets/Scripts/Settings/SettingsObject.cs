using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class SettingsObject : ScriptableObject
    {
        public static T GetObject<T>()
        {
            if (SerializedSettings.AllSettingsObjects.ContainsKey(typeof(T)))
                return (T)SerializedSettings.AllSettingsObjects[typeof(T)];
            return default;
        }

        private void OnEnable()
        {
            if (SerializedSettings.AllSettingsObjects.ContainsKey(GetType()))
            {
                DestroyImmediate((UnityEngine.Object)SerializedSettings.AllSettingsObjects[GetType()]);
                SerializedSettings.AllSettingsObjects.Remove(GetType());
            }
            SerializedSettings.AllSettingsObjects.Add(GetType(), this);
        }
    }
}
