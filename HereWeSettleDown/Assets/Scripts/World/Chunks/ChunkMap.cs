namespace World.Chunks
{
    public static class ChunkMap
    {
        // Copy from Generator.Chunks.ChunkGenerator
        public static int chunkWidth;
        public static int chunkHeight;

        public static float realChunkWidth;
        public static float realChunkHeight;

        public static Chunk[,] chunkMap { get; private set; }

        public static void Set(Chunk[,] chunkMap)
        {
            ChunkMap.chunkMap = chunkMap;
        }
    }
}
