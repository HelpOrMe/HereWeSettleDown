using UnityEngine;
using XNode;
using World.Generator.Nodes.HeightMap.Other;
using Helper.Random;

namespace World.Generator.Nodes.HeightMap
{
    [CreateAssetMenu(fileName ="Height Map Generation", menuName ="Nodes/HeightMapGeneration")]
    public class HeightMapGenerationGraph : NodeGraph 
    {
        public System.Random prng = Seed.prng;
        
        public int mapWidth = 256;
        public int mapHeight = 256;

        public MapRequester requester;

        public float[,] GetMap(int width, int height)
        {
            mapWidth = width;
            mapHeight = height;
            return requester.GetHeightMap().map;
        }
    }
}

