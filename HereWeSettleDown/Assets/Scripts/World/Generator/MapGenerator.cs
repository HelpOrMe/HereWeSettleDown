using UnityEngine;
using World.Map;
using Helper.Threading;
using Helper.Debugger;
using Helper.Random;
using Settings;
using Settings.Generator;
using World.Generator.Nodes.WorldGenerator;

namespace World.Generator
{
    public class MapGenerator : MonoBehaviour
    {
        public ChunkObject chunkObject;
        public WorldGeneratorGraph generatorGraph;
        public int testSeed = 0;

        public void GenerateMap()
        {
            Seed.seed = testSeed;
            
            BaseGeneratorSettings set = SettingsObject.GetObject<BaseGeneratorSettings>();

            Watcher.WatchRun(() => WorldMesh.CreateWorldMesh(set.worldWidth, set.worldHeight, set.chunkWidth, set.chunkHeight), "CreateWorldMesh");
            Watcher.WatchRun(() => WorldChunkMap.CreateMap(set.worldWidth, set.worldHeight, set.chunkWidth, set.chunkHeight, chunkObject.transform.localScale), "CreateMap");

            // Voronoi > Regions > Calculate triangles
            // WaterMask > SetWater > SetCoastline > Set distances
            // Set lakes
            // Set height > Smooth height
            // Set wet
            // Draw colors > smooth colors

            AThread thread = new AThread(generatorGraph.GetGenerateAction());
            thread.Start();
            thread.RunAfterThreadEnd(() => WorldChunkMap.CreateChunks(chunkObject, transform, true));
        }
    }
}
