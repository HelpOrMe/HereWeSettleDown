using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class SerializedSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        public static AllSettingsObjectsDictionary AllSettingsObjects = new AllSettingsObjectsDictionary();
        [SerializeField] private AllSettingsObjectsDictionary allSettingsObject = new AllSettingsObjectsDictionary();

        public void OnBeforeSerialize()
        {
            if (AllSettingsObjects.Count > 0)
            {
                allSettingsObject = AllSettingsObjects;
            }
        }

        public void OnAfterDeserialize()
        {
            AllSettingsObjects = allSettingsObject;
        }
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
                try { Add(Type.GetType($"Settings.{keys[i]}, Assembly-CSharp", false), values[i]); }
                catch { }
            }
        }
    }
}
