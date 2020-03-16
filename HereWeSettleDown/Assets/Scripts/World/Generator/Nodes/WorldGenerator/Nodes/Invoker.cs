using System.Linq;
using XNode;
using Helper.Threading;

namespace World.Generator.Nodes
{
    [NodeWidth(300)]
    public class Invoker : Node
    {
        [Output] public AThread threadAction;

        public void AddActionLayer()
        {
            AddDynamicInput(typeof(System.Action), fieldName: "Action #" + (DynamicInputs.Count() + 1).ToString());
        }

        public void RemoveEmptyActionLayers()
        {
            foreach (var port in DynamicInputs.ToArray())
            {
                if (port.ConnectionCount == 0)
                {
                    RemoveDynamicPort(port.fieldName);
                }
            }
        }

        public void RemoveLastActionLayer()
        {
            if (DynamicInputs.Count() > 0)
            {
                RemoveDynamicPort(DynamicInputs.Last());
            }
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}
