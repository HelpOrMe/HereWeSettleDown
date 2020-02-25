using UnityEngine;
using XNode;
using World.Generator.Helper;

namespace Nodes.HeightMapGeneration.Maps
{
    public class NoiseMap : Node
    {
        public float scale = 25;
        public int octaves = 4;
        public float persistance = 0.5f;
        public float lacunarity = 2;
        [Output] public HeightMap outMap;

        public HeightMap GetOutMap()
        {
            var ghp = (HeightMapGenerationGraph)graph;
            float[,] outMap = Noise.GenerateNoiseMap
                (ghp.prng, ghp.mapWidth, ghp.mapHeight,
                GetInputValue("scale", scale), GetInputValue("octaves", octaves),
                GetInputValue("persistance", persistance), GetInputValue("lacunarity",
                lacunarity), Vector2.zero);

            return new HeightMap(outMap);
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMap")
                return GetOutMap();
            return null;
        }
    }
}
