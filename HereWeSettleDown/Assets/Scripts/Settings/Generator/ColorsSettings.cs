using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings.Generator
{
    [CreateAssetMenu(menuName = "Settings/Generator/ColorsSettings")]
    public class ColorsSettings : SettingsObject
    {
        public BiomesSettings biomesSettings;
        public List<float> heightLayers = new List<float>() { .1f, .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f, 1f };
        public BiomeColorsDictionary biomeColors = new BiomeColorsDictionary();

        [Serializable]
        public class BiomeColorsDictionary : Dictionary<string, BiomeColors>, ISerializationCallbackReceiver
        {
            [SerializeField] private List<string> keys = new List<string>();
            [SerializeField] private List<BiomeColors> values = new List<BiomeColors>();

            public void OnBeforeSerialize()
            {
                keys.Clear();
                values.Clear();
                foreach (KeyValuePair<string, BiomeColors> pair in this)
                {
                    keys.Add(pair.Key);
                    values.Add(pair.Value);
                }
            }

            public void OnAfterDeserialize()
            {
                Clear();
                for (int i = 0; i < keys.Count; i++)
                {
                    Add(keys[i], values[i]);
                }
            }
        }
    }

    [Serializable]
    public class BiomeColors
    {
        public Color waterColor;
        public Color[] heightColors = new Color[1];
    }
}
