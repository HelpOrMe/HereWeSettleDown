using UnityEngine;
using CustomVariables;

namespace World.Map
{
    public static class WorldChunkMap
    {
        static readonly SetOnce<int> chunkWidth = new SetOnce<int>();
        public static int ChunkWidth
        {
            get
            {
                return chunkWidth;
            }
            set
            {
                chunkWidth.Value = value;
            }
        }
        
        static readonly SetOnce<int> chunkHeight = new SetOnce<int>();
        public static int ChunkHeight
        {
            get
            {
                return chunkHeight;
            }
            set
            {
                chunkHeight.Value = value;
            }
        }

        static readonly SetOnce<Vector3> chunkScale = new SetOnce<Vector3>();
        public static Vector3 ChunkScale
        {
            get
            {
                return chunkScale;
            }
            set
            {
                chunkScale.Value = value;
            }
        }

        public static int mapWidth
        {
            get
            {
                return chunkMap.GetLength(0);
            }
        }
        public static int mapHeight
        {
            get
            {
                return chunkMap.GetLength(1);
            }
        }

        public static float WorldChunkWidth
        {
            get
            {
                return chunkWidth * chunkScale.Value.x;
            }
        }
        public static float WorldChunkHeight
        {
            get
            {
                return chunkHeight * chunkScale.Value.z;
            }
        }

        public static Chunk[,] chunkMap;

        public static Chunk GetChunk(Vector2Int mapPosition)
        {
            return chunkMap[
                Mathf.RoundToInt(mapPosition.x / chunkWidth),
                Mathf.RoundToInt(mapPosition.y / chunkHeight)];
        }

        public static Chunk GetChunk(int mapX, int mapY)
        {
            return chunkMap[
                Mathf.RoundToInt(mapX / chunkWidth),
                Mathf.RoundToInt(mapY / chunkHeight)];
        }
    }
}
