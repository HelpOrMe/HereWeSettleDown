using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class SubGenerator : MonoBehaviour
    {
        public static Dictionary<string, object> values = new Dictionary<string, object>();
        private GeneratorRegData regData;

        public GeneratorRegData _registrate()
        {
            OnRegistrate();

            object[] attributes = GetType().GetCustomAttributes(typeof(WorldGenerator), false);
            if (attributes.Length > 0)
            {
                WorldGenerator gen = (WorldGenerator)attributes[0];
                regData = gen.data;
            }
            else
            {
                regData = new GeneratorRegData()
                {
                    priority = 0,
                    requireValues = new string[] { "Nothing" }
                };
            }

            return regData;
        }

        public virtual void OnRegistrate() { }

        public virtual void OnGenerate() { }

        protected void GenerationCompleted()
        {
            Debug.Log(GetType().Name + " completed!");
            MasterGenerator.SetGeneratorState(this, true);
        }

        public static T GetValue<T>(string name)
        {
            if (values.ContainsKey(name))
            {
                return (T)values[name];
            }
            else
            {
                Debug.LogError("Something is trying to get a nonexistent variable " + name);
            }
            return default;
        }

        public static bool TryGetValue<T>(string name, out T value)
        {
            if (values.ContainsKey(name))
            {
                value = (T)values[name];
                return true;
            }

            value = default;
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class WorldGenerator : Attribute
    {
        public GeneratorRegData data;

        public WorldGenerator(int priority = 0, params string[] requireValues)
        {
            data = new GeneratorRegData()
            {
                priority = priority,
                requireValues = requireValues
            };
        }
    }

    public struct GeneratorRegData
    {
        public int priority;
        public string[] requireValues;
    }
}

