using System.Collections.Generic;
using UnityEngine;

namespace World.Chunks
{
    public static class ChunkMeshMap
    {
        public static Vector3[,] verticesMap;
        public static ColorPack[,] colorMap;

        private static readonly Dictionary<Chunk, List<Vector2Int>> editedChunks = new Dictionary<Chunk, List<Vector2Int>>();

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
            int chunkX = x / ChunkMap.ChunkWidth;
            int chunkY = y / ChunkMap.ChunkHeight;
            
            Chunk targetChunk = ChunkMap.chunkMap[chunkX, chunkY];
            if (!editedChunks.ContainsKey(targetChunk))
                editedChunks[targetChunk] = new List<Vector2Int>();
            editedChunks[targetChunk].Add(new Vector2Int(x % (ChunkMap.ChunkWidth - 1), y % (ChunkMap.ChunkHeight - 1)));
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
