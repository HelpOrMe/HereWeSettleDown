using UnityEngine;
using XNode;
using Nodes.HeightMapGeneration.Other;

namespace Nodes.HeightMapGeneration
{
    [CreateAssetMenu(fileName ="Height Map Generation", menuName ="Nodes/HeightMapGeneration")]
    public class HeightMapGenerationGraph : NodeGraph 
    {
        public int editorSeed = 0;
        public System.Random prng
        {
            get
            {
                if (setPrng == null)
                    return new System.Random(editorSeed);
                return setPrng;
            }
        }
        public System.Random setPrng = null;

        public int mapWidth = 256;
        public int mapHeight = 256;

        public MapRequester requester;
    }
}

