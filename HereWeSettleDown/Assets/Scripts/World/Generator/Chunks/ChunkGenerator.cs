using UnityEngine;
using World.Chunks;
using World.Mesh;

namespace World.Generator.Chunks
{
    [CustomGenerator(6, false, "mapWidth", "mapHeight", "chunkMeshDataMap")]
    public class ChunkGenerator : SubGenerator
    {
        public int chunkWidth;
        public int chunkHeight;

        public bool HideAllChunksOnGenerate;
        public GameObject chunkTerrain;

        public override void OnRegistrate()
        {
            values["chunkWidth"] = chunkWidth;
            values["chunkHeight"] = chunkHeight;

            ChunkMap.chunkWidth = chunkWidth;
            ChunkMap.chunkHeight = chunkHeight;

            Vector3 size = chunkTerrain.transform.localScale;
            ChunkMap.realChunkWidth = chunkWidth * size.x;
            ChunkMap.realChunkHeight = chunkHeight * size.y;
        }

        public override void OnGenerate()
        {
            GenerateChunkMap();
            GenerationCompleted();
        }

        public void GenerateChunkMap()
        {
            int mapWidth = GetValue<int>("mapWidth");
            int mapHeight = GetValue<int>("mapHeight");

            ChunkMeshData[,] chunkMeshDataMap = GetValue<ChunkMeshData[,]>("chunkMeshDataMap");
            Chunk[,] chunkMap = new Chunk[mapWidth / chunkWidth, mapHeight / chunkHeight];
            for (int x = 0; x < chunkMap.GetLength(0); x++)
            {
                for (int y = 0; y < chunkMap.GetLength(1); y++)
                {
                    chunkMap[x, y] = new Chunk(chunkTerrain, transform, !HideAllChunksOnGenerate, chunkMeshDataMap[x, y]);
                }
            }

            ChunkMap.Set(chunkMap);
        }
    }
}

