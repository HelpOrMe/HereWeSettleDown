using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Generator.Nodes.WorldGenerator.Base;
using XNode;

namespace World.Generator.Nodes.WorldGenerator
{
    [CreateAssetMenu(fileName = "World Generator Graph", menuName = "Nodes/WorldGeneratorGraph")]
    public class WorldGeneratorGraph : NodeGraph
    {
        public string[] RegisteredGeneratorParts;
        public GeneratorBase generatorBaseNode;

        public Action GetGenerateAction()
        {
            return generatorBaseNode.GetGenerateAction();
        }

        public Type[] GetPartTypes()
        {
            string[] ignored = new string[] { "GeneratorPart", "CombinedPart" };

            List<Type> types = new List<Type>();
            if (RegisteredGeneratorParts != null)
            {
                foreach (string typeName in RegisteredGeneratorParts)
                {
                    if (ignored.Contains(typeName))
                    {
                        continue;
                    }

                    Type type = Type.GetType($"World.Generator.{typeName}, Assembly-CSharp", false);
                    if (type != null && !types.Contains(type))
                    {
                        types.Add(type);
                    }
                }
            }
            return types.ToArray();
        }
    }
}
