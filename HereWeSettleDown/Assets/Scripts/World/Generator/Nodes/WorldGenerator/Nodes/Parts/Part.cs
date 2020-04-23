using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace World.Generator.Nodes.WorldGenerator.Parts
{
    public class Part : Node
    {
        [HideInInspector] public PartObjectFieldsDictionary partObjectFields = new PartObjectFieldsDictionary();

        public bool NewThread = false;
        [HideInInspector] public int selectedPart;
        [Output] public GeneratorPart genPart;

        public object GetPartObject()
        {
            WorldGeneratorGraph wGraph = (WorldGeneratorGraph)graph;
            Type[] types = wGraph.GetPartTypes();
            Type type = types[selectedPart];

            GeneratorPart part = (GeneratorPart)Activator.CreateInstance(type);
            part.RunInNewThread = NewThread;

            foreach (string fieldName in partObjectFields.Keys)
                type.GetField(fieldName).SetValue(part, partObjectFields[fieldName]);

            return part;
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "genPart")
                return GetPartObject();
            return null;
        }

        [Serializable]
        public class PartObjectFieldsDictionary : Dictionary<string, UnityEngine.Object>, ISerializationCallbackReceiver
        {
            [SerializeField] private List<string> keys = new List<string>();
            [SerializeField] private List<UnityEngine.Object> values = new List<UnityEngine.Object>();

            public void OnBeforeSerialize()
            {
                keys.Clear();
                values.Clear();
                foreach (KeyValuePair<string, UnityEngine.Object> pair in this)
                {
                    keys.Add(pair.Key);
                    values.Add(pair.Value);
                }
            }

            public void OnAfterDeserialize()
            {
                Clear();
                for (int i = 0; i < keys.Count; i++)
                    Add(keys[i], values[i]);
            }
        }
    }
}
