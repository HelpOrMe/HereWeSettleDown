using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World.Generator.Chunks;

namespace World.Generator.Flora
{
    [CustomGenerator(false, typeof(ChunkGenerator))]
    public class FloraGenerator : SubGenerator
    {
        public FloraGenerationSettings[] floraGenSettings;

        public override void OnRegistrate()
        {
            values["floraGenSettings"] = floraGenSettings;
        }

        public override void OnGenerate()
        {
            GenerateFloraMasks();
            GenerationCompleted();
        }

        public void GenerateFloraMasks()
        {
            
        }
    }
}
