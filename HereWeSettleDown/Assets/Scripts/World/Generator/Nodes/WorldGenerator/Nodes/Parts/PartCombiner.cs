using System;
using System.Collections.Generic;
using System.Linq;
using XNode;

namespace World.Generator.Nodes.WorldGenerator.Parts
{
    [NodeWidth(300)]
    public class PartCombiner : Node
    {
        [Output] public GeneratorPart combinedPart;

        public void AddPartLayer()
        {
            AddDynamicInput(typeof(GeneratorPart), fieldName: "Part #" + (DynamicInputs.Count() + 1).ToString());
        }

        public void RemoveEmptyPartLayers()
        {
            foreach (NodePort port in DynamicInputs.ToArray())
            {
                if (port.ConnectionCount == 0)
                {
                    RemoveDynamicPort(port.fieldName);
                }
            }
        }

        public void RemoveLastPartLayer()
        {
            if (DynamicInputs.Count() > 0)
            {
                RemoveDynamicPort(DynamicInputs.Last());
            }
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "combinedPart")
            {
                List<GeneratorPart> parts = new List<GeneratorPart>();
                foreach (NodePort dPort in DynamicInputs)
                {
                    GeneratorPart part = (GeneratorPart)dPort.GetInputValue();
                    if (part != null)
                    {
                        parts.Add(part);
                    }
                }
                return new CombinedPart(parts.ToArray());
            }
            return null;
        }
    }

    [Serializable]
    public class CombinedPart : GeneratorPart
    {
        private readonly GeneratorPart[] parts;

        public CombinedPart(GeneratorPart[] parts)
        {
            this.parts = parts;
        }

        protected override void Run()
        {
            foreach (GeneratorPart part in parts)
            {
                part.GetAction()();
            }
        }
    }
}
