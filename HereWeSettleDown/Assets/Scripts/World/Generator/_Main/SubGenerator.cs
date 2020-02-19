using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.Generator
{
    public class SubGenerator : MonoBehaviour
    {
        public static Dictionary<string, object> values = new Dictionary<string, object>();
        public System.Random ownPrng;

        public virtual void OnRegistrate() { }
        public virtual void OnGenerate() { }

        protected void GenerationCompleted()
        {
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
    public sealed class CustomGenerator : Attribute
    {
        public GeneratorRegData data;

        public CustomGenerator(int priority = 0, bool useOwnThread = false, params string[] requireValues)
        {
            data = new GeneratorRegData()
            {
                priority = priority,
                useThread = useOwnThread,
                requireValues = requireValues
            };
        }

        public CustomGenerator(int priority = 0, bool useOwnThread = false, params Type[] requiredCompletedGenerators)
        {
            data = new GeneratorRegData()
            {
                priority = priority,
                useThread = useOwnThread,
                requiredCompletedGenerators = requiredCompletedGenerators
            };
        }
    }

    public struct GeneratorRegData
    {
        public int priority;
        public bool useThread;
        public string[] requireValues;
        public Type[] requiredCompletedGenerators;
    }
}

