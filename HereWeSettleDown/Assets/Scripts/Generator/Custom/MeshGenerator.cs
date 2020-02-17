using UnityEngine;
using World.MeshSystem;

namespace Generator.Custom
{
    [WorldGenerator(7, true, true, "mapWidth", "mapHeight", "chunkWidth", "chunkHeight", "heightMap", "colorMap")]
    public class MeshGenerator : SubGenerator
    {
        public Vector2Int triangleRangeXScale;
        public Vector2Int triangleRangeYScale;
        public int verticesHeightScale;
        public AnimationCurve verticesCurve;

        public override void OnGenerate()
        {
            GenerateChunkMeshes();
            GenerationCompleted();
        }

        public void GenerateChunkMeshes()
        {
            int width = GetValue<int>("mapWidth");
            int height = GetValue<int>("mapHeight");

            float[,] heightMap = GetValue<float[,]>("heightMap");
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

            MeshMap.SetVerticesMap(verticesMap);
            MeshMap.SetColorMap(GetValue<ColorPack[,]>("colorMap"));

            // Create all chunksMeshes
            ChunkMeshData[,] chunkMeshDataMap = new ChunkMeshData[chunkXCount, chunkYCount];
            for (int cX = 0; cX < chunkXCount; cX++)
            {
                for (int cY = 0; cY < chunkYCount; cY++)
                {
                    chunkMeshDataMap[cX, cY] = new ChunkMeshData(chunkWidth - 1, chunkHeight - 1, cX, cY);
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
                    offset[x, y] = new Vector3(x, 0, y) + new Vector3(ownPrng.Next(triangleRangeXScale.x, triangleRangeXScale.y) / 100f, 0, ownPrng.Next(triangleRangeYScale.x, triangleRangeYScale.y) / 100f);
                }
            }

            return offset;
        }

        private float EvaluteVerticesHeight(float height)
        {
            return verticesCurve.Evaluate(height) * verticesHeightScale;
        }
    }
}

