using UnityEngine;
using XNode;
using World.Generator.Nodes.HeightMap.Other;

namespace World.Generator.Nodes.HeightMap
{
    [CreateAssetMenu(fileName ="Height Map Generation", menuName ="Nodes/HeightMapGeneration")]
    public class HeightMapGenerationGraph : NodeGraph 
    {
        public System.Random prng
        {
            get
            {
                // If try generate from editor
                if (_prng == null)
                    return new System.Random(0);
                return _prng;
            }
        }
        private System.Random _prng;


        public int mapWidth = 256;
        public int mapHeight = 256;

        public MapRequester requester;

        public float[,] GetMap(int width, int height, System.Random prng)
        {
            _prng = prng;
            mapWidth = width;
            mapHeight = height;
            return requester.GetHeightMap().map;
        }
    }
}

