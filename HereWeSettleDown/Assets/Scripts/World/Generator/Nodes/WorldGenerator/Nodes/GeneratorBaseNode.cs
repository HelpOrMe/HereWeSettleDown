using UnityEngine;
using XNode;
using Helper.Threading;

namespace World.Generator.Nodes
{
    public class GeneratorBaseNode : Node
    {
        [Input] public AThread GenerateAction;
        //[Input] public GeneratorSettings settings;


        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
    }
}
