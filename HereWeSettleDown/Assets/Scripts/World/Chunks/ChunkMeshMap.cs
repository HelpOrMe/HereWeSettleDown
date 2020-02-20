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
            Chunk targetChunk = ChunkMap.GetChunk(x, y);
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

        public static Vector3 GetNearVertexPos(Vector3 worldPosition)
        {
            Vector2Int vertPosition = TranslatePosition(worldPosition);
            return verticesMap[vertPosition.x, vertPosition.y];
        }

        public static ColorPack GetNearColor(Vector3 worldPosition)
        {
            Vector2Int vertPosition = TranslatePosition(worldPosition);
            return colorMap[vertPosition.x, vertPosition.y];
        }

        public static Vector2Int TranslatePosition(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.Clamp(Mathf.RoundToInt(worldPosition.x / ChunkMap.ChunkScale.x), 0, verticesMap.GetLength(0) - 1),
                 Mathf.Clamp(Mathf.RoundToInt(worldPosition.z / ChunkMap.ChunkScale.z), 0, verticesMap.GetLength(1) - 1));
        }
    }
}
