using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class SettingsObject : ScriptableObject
    {
        private void OnEnable()
        {
            if (!SerializedSettings.AllSettingsObjects.ContainsKey(GetType()))
            {
                // DestroyImmediate((UnityEngine.Object)SerializedSettings.AllSettingsObjects[GetType()]);
                // SerializedSettings.AllSettingsObjects.Remove(GetType());
                SerializedSettings.AllSettingsObjects.Add(GetType(), this);
            }
        }
    }
}
