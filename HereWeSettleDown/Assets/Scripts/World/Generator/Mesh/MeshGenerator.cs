using UnityEngine;
using World.Chunks;

namespace World.Generator.Mesh
{
    [CustomGenerator(true, "mapWidth", "mapHeight", "chunkWidth", "chunkHeight", "biomedHeightMap", "colorMap")]
    public class MeshGenerator : SubGenerator
    {
        public Vector2Int triangleRangeScale;
        public int verticesHeightScale;
        public AnimationCurve verticesHeightCurve;

        public override void OnGenerate()
        {
            GenerateChunkMeshes();
            GenerationCompleted();
        }

        public void GenerateChunkMeshes()
        {
            int width = GetValue<int>("mapWidth");
            int height = GetValue<int>("mapHeight");

            float[,] heightMap = GetValue<float[,]>("biomedHeightMap");
            Vector3[,] offsetMap = GenerateOffset(width, height);

            int chunkWidth = GetValue<int>("chunkWidth");
            int chunkHeight = GetValue<int>("chunkHeight");

            int chunkXCount = width / chunkWidth;
            int chunkYCount = height / chunkHeight;

            // Create vertices map
            Vector3[,] verticesMap = new Vector3[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    verticesMap[x, y] = offsetMap[x, y] + Vector3.up * EvaluteVerticesHeight(heightMap[x, y]);
                }
            }

            ChunkMeshMap.verticesMap = verticesMap;
            ChunkMeshMap.colorMap = GetValue<ColorPack[,]>("colorMap");

            // Create all chunksMeshes
            ChunkMeshData[,] chunkMeshDataMap = new ChunkMeshData[chunkXCount, chunkYCount];
            for (int x = 0; x < chunkXCount; x++)
            {
                for (int y = 0; y < chunkYCount; y++)
                {
                    chunkMeshDataMap[x, y] = new ChunkMeshData(chunkWidth - 1, chunkHeight - 1, x, y);
                }
            }

            values["chunkMeshDataMap"] = chunkMeshDataMap;
        }

        private Vector3[,] GenerateOffset(int width, int height)
        {
            Vector3[,] offset = new Vector3[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    offset[x, y] = new Vector3(x, 0, y) + new Vector3(ownPrng.Next(triangleRangeScale.x, triangleRangeScale.y) / 100f, 0, ownPrng.Next(triangleRangeScale.x, triangleRangeScale.y) / 100f);
                }
            }

            return offset;
        }

        private float EvaluteVerticesHeight(float height)
        {
            return verticesHeightCurve.Evaluate(height) * verticesHeightScale;
        }
    }
}

