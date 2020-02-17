namespace World.ChunkSystem
{
    public static class ChunkMap
    {
        public static int chunkWidth;
        public static int chunkHeight;

        public static Chunk[,] chunkMap { get; private set; }

        public static void Set(Chunk[,] chunkMap)
        {
            ChunkMap.chunkMap = chunkMap;
        }
    }
}
