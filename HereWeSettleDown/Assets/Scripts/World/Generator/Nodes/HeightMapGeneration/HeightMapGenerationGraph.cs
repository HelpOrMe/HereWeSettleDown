using UnityEngine;
using World.Generator.Nodes.HeightMap.Other;
using XNode;

namespace World.Generator.Nodes.HeightMap
{
    [CreateAssetMenu(fileName = "Height Map Generation", menuName = "Nodes/HeightMapGeneration")]
    public class HeightMapGenerationGraph : NodeGraph
    {
        [SerializeField] private int editorSeed = 0;

        public System.Random prng
        {
            get
            {
                // If run from an editor
                if (_prng == null)
                    return new System.Random(editorSeed);
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

