using Helper.Debugging;
using Helper.Random;
using Helper.Threading;
using Settings;
using Settings.Generator;
using UnityEngine;
using World.Generator.Nodes.WorldGenerator;
using World.Map;


namespace World.Generator
{
    public class MapGenerator : MonoBehaviour
    {
        public ChunkObject chunkObject;
        public WorldGeneratorGraph generatorGraph;
        public int testSeed = 0;

        public void GenerateMap()
        {
            Log.SetWorker("GenerateMap");

            Seed.seed = testSeed;
            Log.InfoSet("Seed");

            BaseGeneratorSettings set = GameSettingsProvider.GetSettings<BaseGeneratorSettings>();

            Watcher.WatchRun(() => WorldMesh.CreateWorldMesh(set.worldWidth, set.worldHeight, set.chunkWidth, set.chunkHeight), "CreateWorldMesh");
            Watcher.WatchRun(() => WorldChunkMap.CreateMap(set.worldWidth, set.worldHeight, set.chunkWidth, set.chunkHeight, chunkObject.transform.localScale), "CreateMap");

            // Voronoi > Regions > Calculate triangles
            // WaterMask > SetWater > SetCoastline > Set distances
            // Set rivers
            // Set height > Smooth height
            // Set moisture
            // Draw colors > smooth colors

            Log.ResetWorker();

            AThread thread = new AThread(generatorGraph.GetGenerateAction());
            thread.Start();
            thread.RunAfterThreadEnd(() => WorldChunkMap.CreateChunks(chunkObject, transform, true));
        }
    }
}
