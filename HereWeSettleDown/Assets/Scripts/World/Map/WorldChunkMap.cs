using UnityEngine;

namespace World.Map
{
    public static class WorldChunkMap
    {
        // In quads
        public static int worldWidth { get; private set; }
        public static int worldHeight { get; private set; }

        public static int chunkWidth { get; private set; }
        public static int chunkHeight { get; private set; }

        public static int chunkXCount { get; private set; }
        public static int chunkYCount { get; private set; }

        public static Vector3 chunkScale { get; private set; }

        public static Chunk[,] chunkMap;

        public static void CreateMap(int worldWidth, int worldHeight, int chunkWidth, int chunkHeight, Vector3 chunkScale)
        {
            WorldChunkMap.worldWidth = worldWidth;
            WorldChunkMap.worldHeight = worldHeight;

            WorldChunkMap.chunkWidth = chunkWidth;
            WorldChunkMap.chunkHeight = chunkHeight;

            WorldChunkMap.chunkScale = chunkScale;

            chunkXCount = worldWidth / chunkWidth;
            chunkYCount = worldHeight / chunkHeight;
        }

        public static void CreateChunks(ChunkObject terrain, Transform parent, bool visible)
        {
            if (WorldMesh.chunkMeshMap == null)
            {
                return;
            }

            WorldMesh.ConfirmChanges(false);
            ClearChunkMap();

            chunkMap = new Chunk[chunkXCount, chunkYCount];
            for (int x = 0; x < chunkXCount; x++)
            {
                for (int y = 0; y < chunkYCount; y++)
                {
                    chunkMap[x, y] = new Chunk(terrain, parent, visible, WorldMesh.chunkMeshMap[x, y]);
                }
            }
        }

        private static void ClearChunkMap()
        {
            if (chunkMap == null)
            {
                return;
            }

            for (int x = 0; x < chunkXCount; x++)
            {
                for (int y = 0; y < chunkYCount; y++)
                {
                    Object.Destroy(chunkMap[x, y].chunkObject.gameObject);
                }
            }
        }

        public static Chunk GetChunk(Vector2Int mapPosition)
        {
            return GetChunk(mapPosition.x, mapPosition.y);
        }

        public static Chunk GetChunk(int mapX, int mapY)
        {
            return chunkMap[
                Mathf.RoundToInt(mapX / chunkWidth),
                Mathf.RoundToInt(mapY / chunkHeight)];
        }
    }
}
