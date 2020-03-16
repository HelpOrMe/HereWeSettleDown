using System;
using UnityEngine;
using XNode;

namespace World.Generator.Nodes
{
    public class ActionNode : Node
    {
        public GeneratorPart part;
        [Output] public Action action;

        protected override void Init()
        {

        }

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}
