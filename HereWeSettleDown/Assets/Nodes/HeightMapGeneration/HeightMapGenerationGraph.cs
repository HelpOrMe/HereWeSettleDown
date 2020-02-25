using UnityEngine;
using XNode;

namespace Nodes.HeightMapGeneration
{
    [CreateAssetMenu(fileName ="Height Map Generation", menuName ="Nodes/HeightMapGeneration")]
    public class HeightMapGenerationGraph : NodeGraph 
    {
        public System.Random prng
        {
            get
            {
                if (setPrng == null)
                    return new System.Random(0);
                return setPrng;
            }
        }
        public System.Random setPrng = null;

        public int mapWidth = 256;
        public int mapHeight = 256;
    }
}

