using System;
using UnityEngine;
using XNode;

namespace World.Generator.Nodes.WorldGenerator.Base
{
    public class GeneratorBase : Node
    {
        [Input] public GeneratorPart generatePart;

        protected override void Init()
        {
            ((WorldGeneratorGraph)graph).generatorBaseNode = this;
        }

        public Action GetGenerateAction()
        {
            return GetInputValue<GeneratorPart>("generatePart").GetAction();
        }
    }
}

