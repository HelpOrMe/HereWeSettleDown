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
            Seed.seed = testSeed;
            Log.InfoSet("Seed");

            BaseGeneratorSettings set = SerializedSettings.GetSettings<BaseGeneratorSettings>();

            Watcher.WatchRun(() => WorldMesh.CreateWorldMesh(set.worldWidth, set.worldHeight, set.chunkWidth, set.chunkHeight), "CreateWorldMesh");
            Watcher.WatchRun(() => WorldChunkMap.CreateMap(set.worldWidth, set.worldHeight, set.chunkWidth, set.chunkHeight, chunkObject.transform.localScale), "CreateMap");

            // Voronoi > Regions > Calculate triangles
            // WaterMask > SetWater > SetCoastline > Set distances
            // Set rivers
            // Set height > Smooth height
            // Set moisture
            // Draw colors > smooth colors

            AThread thread = new AThread(generatorGraph.GetGenerateAction());
            thread.Start();
            thread.RunAfterThreadEnd(() => WorldChunkMap.CreateChunks(chunkObject, transform, true));
        }

        public void Debug_DrawMoisture()
        {
            foreach (Region region in RegionsInfo.regions)
            {
                Color targetColor = Color.Lerp(Color.white, Color.black, (float)region.type.Moisture / RegionsInfo.MaxMoistureIndex);
                region.DoForEachPosition(pos => WorldMesh.colorMap[pos.x, pos.y].ALL = targetColor);
            }
            WorldMesh.ConfrimChangeSplitted();
        }

        public void Debug_DrawHeight()
        {
            foreach (Region region in RegionsInfo.regions)
            {
                Color targetColor = Color.Lerp(Color.black, Color.white, (float)region.type.DistIndexFromCoastline / RegionsInfo.MaxDistIndex);
                region.DoForEachPosition(pos => WorldMesh.colorMap[pos.x, pos.y].ALL = targetColor);
            }
            WorldMesh.ConfrimChangeSplitted();
        }
    }
}
