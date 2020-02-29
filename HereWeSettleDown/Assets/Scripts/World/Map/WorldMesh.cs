using System.Collections.Generic;
using UnityEngine;

namespace World.Map
{
    public static class WorldMesh
    {
        // In quads
        private static int mapWidth, mapHeight;
        private static int chunkWidth, chunkHeight;
        private static int chunkXCount, chunkYCount;

        public static VerticesMap verticesMap;
        public static ColorMap colorMap;
        
        public static ChunkMesh[,] chunkMeshMap;

        private static readonly Dictionary<ChunkMesh, List<Vector2Int>> editedChunks = new Dictionary<ChunkMesh, List<Vector2Int>>();

        public static void CreateWorldMesh(int width, int height, int chunkWidth, int chunkHeight)
        {
            mapWidth = width; 
            mapHeight = height;

            WorldMesh.chunkWidth = chunkWidth;
            WorldMesh.chunkHeight = chunkHeight;

            chunkXCount = width / chunkWidth;
            chunkYCount = height / chunkHeight;

            SetEmptyVerticesMap();
            SetEmptyColorMap();

            chunkMeshMap = new ChunkMesh[chunkXCount, chunkYCount];
            for (int x = 0; x < chunkXCount; x++)
            {
                for (int y = 0; y < chunkYCount; y++)
                {
                    chunkMeshMap[x, y] = new ChunkMesh(chunkWidth, chunkHeight, x, y);
                }
            }
        }

        public static void SetEmptyVerticesMap()
        {
            Vector3[,] verticesPosMap = new Vector3[mapWidth + 1, mapHeight + 1];
            for (int x = 0; x < mapWidth + 1; x++)
            {
                for (int y = 0; y < mapHeight + 1; y++)
                {
                    verticesPosMap[x, y] = new Vector3(x, 0, y);
                }
            }
            verticesMap = new VerticesMap(verticesPosMap);
        }

        public static void SetEmptyColorMap()
        {
            ColorQuad[,] colorQuadMap = new ColorQuad[mapWidth, mapHeight];
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    colorQuadMap[x, y] = new ColorQuad(x, y);
                }
            }
            colorMap = new ColorMap(colorQuadMap);
        }

        public static void SetEditedPosition(int x, int y)
        {
            ChunkMesh chunkMesh = GetChunkMesh(x, y);
            if (!editedChunks.ContainsKey(chunkMesh))
                editedChunks[chunkMesh] = new List<Vector2Int>();
            editedChunks[chunkMesh].Add(new Vector2Int(x % chunkWidth, y % chunkHeight));
        }

        public static void ConfirmChanges()
        {
            foreach (ChunkMesh chunkMesh in editedChunks.Keys)
            {
                foreach (Vector2Int editedQuad in editedChunks[chunkMesh])
                {
                    chunkMesh.UpdateQuadValues(editedQuad.x, editedQuad.y);
                }
                chunkMesh.UpdateConnectedChunk();
            }
            editedChunks.Clear();
        }

        public static void UpdateAllMesh()
        {
            if (chunkMeshMap == null)
                return;

            for (int x = 0; x < chunkXCount; x++)
            {
                for (int y = 0; y < chunkYCount; y++)
                {
                    chunkMeshMap[x, y].UpdateConnectedChunk();
                }
            }
        }

        public static Vector3 GetNearVertexPos(Vector3 worldPosition)
        {
            Vector2Int vertPosition = WorldToMap(worldPosition);
            return verticesMap[vertPosition.x, vertPosition.y];
        }

        public static ColorQuad GetNearColorQuad(Vector3 worldPosition)
        {
            Vector2Int vertPosition = WorldToMap(worldPosition);
            return colorMap.map[vertPosition.x, vertPosition.y];
        }

        public static Vector2Int WorldToMap(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.Clamp(Mathf.RoundToInt(worldPosition.x / WorldChunkMap.chunkScale.x), 0, verticesMap.width - 1),
                 Mathf.Clamp(Mathf.RoundToInt(worldPosition.z / WorldChunkMap.chunkScale.z), 0, verticesMap.height - 1));
        }

        public static ChunkMesh GetChunkMesh(Vector2Int quadPos)
        {
            return GetChunkMesh(quadPos.x, quadPos.y);
        }

        public static ChunkMesh GetChunkMesh(int quadX, int quadY)
        {
            return chunkMeshMap[
                Mathf.RoundToInt(quadX / chunkWidth),
                Mathf.RoundToInt(quadY / chunkHeight)];
        }

        public static Vector2Int VertexPosToQuadPos(Vector2 pos)
        {
            return new Vector2Int(
                (int)Mathf.Clamp(pos.x, 0, mapWidth - 1),
                (int)Mathf.Clamp(pos.y, 0, mapHeight - 1));
        }
    }
}
