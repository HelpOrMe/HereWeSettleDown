using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using Helper;

namespace Generator
{
    public class MasterGenerator : MonoBehaviour
    {
        public int seed;

        public static bool GeneratorsRegistered { get; private set; }
        public static bool GenerationEnd { get; private set; }

        protected SubGenerator[] registratedGenerators;
        protected Dictionary<SubGenerator, GeneratorRegData> generatorsRegData = new Dictionary<SubGenerator, GeneratorRegData>();
        protected Dictionary<int, List<SubGenerator>> generatorsPriority = new Dictionary<int, List<SubGenerator>>();
        protected static Dictionary<SubGenerator, bool> generatorStates = new Dictionary<SubGenerator, bool>();
        protected Dictionary<SubGenerator, Thread> generatorThreads = new Dictionary<SubGenerator, Thread>();

        protected float generationStartedAt;
        protected float generationEndedAt;

        private void Update()
        {
            if (!GenerationEnd)
            {
                CheckForGenerationEnd();
            }
        }

        public void RegistrateGenerators()
        {
            ClearRegistratedGenerators();
            System.Random mainPrng = new System.Random(seed);

            registratedGenerators = ObjectFinder.FindRootObjects<SubGenerator>();
            foreach (SubGenerator gen in registratedGenerators)
            {
                if (!generatorsRegData.ContainsKey(gen))
                {
                    // Add generator to registered dictionary
                    GeneratorRegData data = GetGeneratorData(gen);
                    generatorsRegData[gen] = data;

                    // Add generator to priority dictionary
                    if (!generatorsPriority.ContainsKey(data.priority))
                        generatorsPriority[data.priority] = new List<SubGenerator>();
                    generatorsPriority[data.priority].Add(gen);

                    // Add generator to state dictionary
                    generatorStates[gen] = false;

                    // Add SubGenerator prng
                    gen.ownPrng = new System.Random(mainPrng.Next(int.MinValue, int.MaxValue));
                    gen.OnRegistrate();
                }
            }
            GeneratorsRegistered = true;
        }

        public GeneratorRegData GetGeneratorData(SubGenerator generator)
        {
            GeneratorRegData regData;

            object[] attributes = generator.GetType().GetCustomAttributes(typeof(WorldGenerator), false);
            if (attributes.Length > 0)
            {
                WorldGenerator genAttr = (WorldGenerator)attributes[0];
                regData = genAttr.data;
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

        public void ClearRegistratedGenerators()
        {
            generatorsRegData.Clear();
            generatorsPriority.Clear();
            generatorStates.Clear();
            GeneratorsRegistered = false;
        }

        public void StartGenerate()
        {
            generationStartedAt = Time.time;

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

        private IEnumerator ActivateGenerator(SubGenerator generator, GeneratorRegData regData)
        {
            yield return new WaitUntil(() => CheckDataValues(regData.requireValues));
            if (regData.useThread)
            {
                // Create own thread to SubGenerator
                Thread thread = new Thread(generator.OnGenerate)
                {
                    IsBackground = true,
                    Name = generator.name
                };

                Debug.Log(generator.name + " in own thread stated.");
                thread.Start();
                generatorThreads[generator] = thread;
            }
            else
            {
                // Run in main Thread
                Debug.Log(generator.name + " in main thread stated.");
                generator.OnGenerate();
            }
        }

        private bool CheckDataValues(string[] requireValues)
        {
            if (requireValues.Contains("Nothing"))
                return true;

            foreach (string requireValue in requireValues)
            {
                if (!SubGenerator.values.ContainsKey(requireValue))
                    return false;
            }

            return true;
        }

        public static void SetGeneratorState(SubGenerator generator, bool state)
        {
            if (GeneratorsRegistered)
            {
                if (generatorStates.ContainsKey(generator))
                {
                    generatorStates[generator] = state;
                    if (state)
                        // You cannot take the name of an object not through the main thread. (c) Unity
                        Debug.Log(generator.GetType().Name + " generation completed."); 
                }
            }
            else
                Debug.LogWarning("Attempt to change the state of the generator before registering it.");
        }

        private void CheckForGenerationEnd()
        {
            if (GeneratorsRegistered && !generatorStates.Values.Contains(false))
            {
                generationEndedAt = Time.time;
                Debug.Log("Time has passed since the start of generation " + (generationEndedAt - generationStartedAt));

                GenerationEnd = true;
                OnGenerationEnd();
            }
        }

        public virtual void OnGenerationEnd() { }
    }
}
