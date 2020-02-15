using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Helper;

namespace Generator
{
    public class MasterGenerator : MonoBehaviour
    {
        public static bool GeneratorsRegistered { get; private set; }
        public static bool GenerationEnd { get; private set; }

        protected SubGenerator[] registratedGenerators;
        protected Dictionary<SubGenerator, GeneratorRegData> generatorsRegData = new Dictionary<SubGenerator, GeneratorRegData>();
        protected Dictionary<int, List<SubGenerator>> generatorsPriority = new Dictionary<int, List<SubGenerator>>();
        protected static Dictionary<SubGenerator, bool> generatorStates = new Dictionary<SubGenerator, bool>();

        private void Update()
        {
            if (!GenerationEnd)
            {
                CheckForGenerationEnd();
            }
        }

        public void RegistrateGenerators()
        {
            ClearRegisteredGenerators();

            registratedGenerators = ObjectFinder.FindRootObjects<SubGenerator>();
            foreach (SubGenerator gen in registratedGenerators)
            {
                if (!generatorsRegData.ContainsKey(gen))
                {
                    // Add generator to registered dictionary
                    GeneratorRegData data = gen._registrate();
                    generatorsRegData[gen] = data;

                    // Add generator to priority dictionary
                    if (!generatorsPriority.ContainsKey(data.priority))
                        generatorsPriority[data.priority] = new List<SubGenerator>();
                    generatorsPriority[data.priority].Add(gen);

                    // Add generator to state dictionary
                    generatorStates[gen] = false;
                }
            }
            GeneratorsRegistered = true;
        }

        public void ClearRegisteredGenerators()
        {
            generatorsRegData.Clear();
            generatorsPriority.Clear();
            generatorStates.Clear();
            GeneratorsRegistered = false;
        }

        public void StartGenerate()
        {
            int[] keys = generatorsPriority.Keys.ToArray();
            Array.Sort(keys);

            foreach (int priority in keys)
            {
                foreach (SubGenerator gen in generatorsPriority[priority])
                {
                    StartCoroutine(ActivateGenerator(gen, generatorsRegData[gen]));
                }
            }
        }

        private IEnumerator ActivateGenerator(SubGenerator generator, GeneratorRegData data)
        {
            yield return new WaitUntil(() => CheckDataValues(data));
            generator.OnGenerate();
        }

        private bool CheckDataValues(GeneratorRegData data)
        {
            bool result = false;

            if (data.requireValues.Contains("Nothing"))
                return true;

            foreach (string requireValue in data.requireValues)
            {
                result = SubGenerator.values.ContainsKey(requireValue);
            }

            return result;
        }

        public static void SetGeneratorState(SubGenerator generator, bool state)
        {
            if (GeneratorsRegistered)
            {
                if (generatorStates.ContainsKey(generator))
                    generatorStates[generator] = state;
            }
            else
                Debug.LogError("Attempt to change the state of the generator before registering it.");
        }

        private void CheckForGenerationEnd()
        {
            if (GeneratorsRegistered && !generatorStates.Values.Contains(false))
            {
                GenerationEnd = true;
                OnGenerationEnd();
            }
        }

        public virtual void OnGenerationEnd() { }
    }
}
