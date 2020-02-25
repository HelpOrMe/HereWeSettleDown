using UnityEngine;
using World.Generator.Mesh;
using World.Map;

namespace World.Generator.Chunks
{
    [CustomGenerator(false, typeof(MeshGenerator))]
    public class ChunkGenerator : SubGenerator
    {
        public int chunkWidth;
        public int chunkHeight;

        public bool HideAllChunksOnGenerate;
        public ChunkObject chunkTerrain;

        public override void OnRegistrate()
        {
            values["chunkWidth"] = chunkWidth;
            values["chunkHeight"] = chunkHeight;

            WorldChunkMap.ChunkWidth = chunkWidth;
            WorldChunkMap.ChunkHeight = chunkHeight;
            WorldChunkMap.ChunkScale = chunkTerrain.transform.localScale;
        }

        public override void OnGenerate()
        {
            GenerateWorldChunkMap();
            GenerationCompleted();
        }

        public void GenerateWorldChunkMap()
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

            WorldChunkMap.chunkMap = chunkMap;
        }
    }
}

