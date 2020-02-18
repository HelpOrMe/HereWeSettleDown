using System.Collections.Generic;
using UnityEngine;
using World.ChunkSystem;
using Generator;

namespace World.MeshSystem
{
    public static class MeshMap
    {
        public static Vector3[,] verticesMap { get; private set; }
        public static ColorPack[,] colorMap { get; private set; }

        private static Dictionary<Chunk, List<Vector2Int>> editedChunks = new Dictionary<Chunk, List<Vector2Int>>();

        public static void SetVerticesMap(Vector3[,] verticesMap)
        {
            MeshMap.verticesMap = verticesMap;
            //...
        }

        public static void SetColorMap(ColorPack[,] colorMap)
        {
            MeshMap.colorMap = colorMap;
            //...
        }

        public static void SetVertex(int x, int y, Vector3 vertexPos)
        {
            if (x >= 0 && x < verticesMap.GetLength(0) &&
                y >= 0 && y < verticesMap.GetLength(1))
            {
                verticesMap[x, y] = vertexPos;
                SetEditedPosition(x, y);
            }
        }

        public static void SetColor(int x, int y, int index, Color color)
        {
            if (x >= 0 && x < colorMap.GetLength(0) &&
                y >= 0 && y < colorMap.GetLength(1))
            {
                colorMap[x, y][index] = color;
                SetEditedPosition(x, y);
            }
        }

        public static void SetEditedPosition(int x, int y)
        {
            if (SubGenerator.TryGetValue("chunkWidth", out int chunkWidth) && 
                SubGenerator.TryGetValue("chunkHeight", out int chunkHeight))
            {
                int chunkX = x / chunkWidth;
                int chunkY = y / chunkHeight;
                if (ChunkMap.chunkMap != null)
                {
                    Chunk targetChunk = ChunkMap.chunkMap[chunkX, chunkY];
                    if (!editedChunks.ContainsKey(targetChunk))
                        editedChunks[targetChunk] = new List<Vector2Int>();
                    editedChunks[targetChunk].Add(new Vector2Int(x % (chunkWidth - 1), y % (chunkHeight - 1)));
                }
            }
        }

        public static void ConfirmChanges()
        {
            foreach (Chunk chunk in editedChunks.Keys)
            {
                foreach (Vector2Int editedQuad in editedChunks[chunk])
                {
                    chunk.meshData.UpdateQuadValues(editedQuad.x, editedQuad.y);
                }
                chunk.DrawMesh();
            }
            editedChunks.Clear();
        }
    }
}
