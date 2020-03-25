using AccidentalNoise;
using System.Threading;
using World.Generator.Helper;
using XNode;

namespace World.Generator.Nodes.HeightMap.Maps
{
    public class AccidentalNoiseMap : Node
    {
        public FractalType fractalType = FractalType.FBM;
        public BasisTypes basisType = BasisTypes.GRADIENT;
        public InterpTypes interpType = InterpTypes.QUINTIC;

        public float scale = 1;
        public int octaves = 4;
        public float frequency = 2f;
        public float lacunarity = 2f;

        [Output] public HeightMap outMap;
        private float[,] outFloatMap;

        public HeightMap GetOutMap()
        {
            Thread thread = new Thread(GenerateMap);
            thread.Start();
            thread.Join();
            return new HeightMap(outFloatMap);
        }

        public void GenerateMap()
        {
            HeightMapGenerationGraph ghp = (HeightMapGenerationGraph)graph;
            AccidentalNoiseSettings settings = new AccidentalNoiseSettings()
            {
                fractalType = fractalType,
                basisType = basisType,
                interpType = interpType,
                scale = scale,
                octaves = octaves,
                frequency = frequency,
                lacunarity = lacunarity
            };
            outFloatMap = Noise.GenerateNoiseMap(ghp.prng, ghp.mapWidth, ghp.mapHeight, settings);
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMap")
            {
                return GetOutMap();
            }

            return null;
        }
    }
}
