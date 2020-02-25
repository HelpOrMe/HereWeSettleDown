using System.Collections.Generic;
using UnityEngine;

namespace World.Map
{
    public static class WorldMeshMap
    {
        public static VerticesMap verticesMap;
        
        public static ColorMap colorMap;

        private static readonly Dictionary<Chunk, List<Vector2Int>> editedChunks = new Dictionary<Chunk, List<Vector2Int>>();

        public static void SetEditedPosition(int x, int y)
        {
            Chunk targetChunk = WorldChunkMap.GetChunk(x, y);
            if (!editedChunks.ContainsKey(targetChunk))
                editedChunks[targetChunk] = new List<Vector2Int>();
            editedChunks[targetChunk].Add(new Vector2Int(x % (WorldChunkMap.ChunkWidth - 1), y % (WorldChunkMap.ChunkHeight - 1)));
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
            return colorMap.map[vertPosition.x, vertPosition.y];
        }

        public static Vector2Int TranslatePosition(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.Clamp(Mathf.RoundToInt(worldPosition.x / WorldChunkMap.ChunkScale.x), 0, verticesMap.width - 1),
                 Mathf.Clamp(Mathf.RoundToInt(worldPosition.z / WorldChunkMap.ChunkScale.z), 0, verticesMap.height - 1));
        }
    }
}
