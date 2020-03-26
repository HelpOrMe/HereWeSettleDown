using UnityEngine;
using World.Generator.Helper;
using XNode;

namespace World.Generator.Nodes.HeightMap.Maps
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
            HeightMapGenerationGraph ghp = (HeightMapGenerationGraph)graph;
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
