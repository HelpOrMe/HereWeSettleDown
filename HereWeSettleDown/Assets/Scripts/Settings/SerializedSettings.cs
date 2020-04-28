using Helper.Debugging;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable, CreateAssetMenu(menuName = "Settings/SerializedSettings")]
    public class SerializedSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        public static AllSettingsObjectsDictionary AllSettingsObjects = new AllSettingsObjectsDictionary();
        [SerializeField] private AllSettingsObjectsDictionary allSettingsObject = new AllSettingsObjectsDictionary();

        private void OnEnable()
        {
            allSettingsObject = AllSettingsObjects;
            allSettingsObject.OnBeforeSerialize();
            allSettingsObject.OnAfterDeserialize();
            AllSettingsObjects = allSettingsObject;
        }

        public static T GetSettings<T>() where T : SettingsObject
        {
            if (AllSettingsObjects.ContainsKey(typeof(T)))
                return (T)AllSettingsObjects[typeof(T)];
            Log.Warning("Settings not found:", typeof(T).Name);
            return default;
        }

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
    public class AllSettingsObjectsDictionary : Dictionary<Type, SettingsObject>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<string> keys = new List<string>();
        [SerializeField] private List<SettingsObject> values = new List<SettingsObject>();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<Type, SettingsObject> pair in this)
            {
                keys.Add(pair.Key.FullName);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                Add(Type.GetType($"{keys[i]}, Assembly-CSharp", false), values[i]);
            }
        }
    }
}
