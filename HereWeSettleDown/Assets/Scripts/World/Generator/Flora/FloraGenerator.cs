using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World.Generator.Chunks;

namespace World.Generator.Flora
{
    [CustomGenerator(8, false, typeof(ChunkGenerator))]
    public class FloraGenerator : SubGenerator
    {
        public FloraType[] floraTypes;

        public override void OnRegistrate()
        {
            values["floraTypes"] = floraTypes;
        }

        public override void OnGenerate()
        {
            GenerateFloraMasks();
            GenerationCompleted();
        }

        public void GenerateFloraMasks()
        {
            for (int i = 0; i < floraTypes.Length; i ++)
            {

            }
        }
    }

    public struct ObjectsFloraMask
    {
        public int[,] mask;
        public FloraType floraType;

        public ObjectsFloraMask(int[,] mask, FloraType floraType)
        {
            this.mask = mask;
            this.floraType = floraType;
        }
    }
}
