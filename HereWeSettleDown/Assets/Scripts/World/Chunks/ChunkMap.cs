using CustomVariables;

namespace World.Chunks
{
    public static class ChunkMap
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

        static readonly SetOnce<float> worldChunkWidth = new SetOnce<float>();
        public static float WorldChunkWidth
        {
            get
            {
                return worldChunkWidth;
            }
            set
            {
                worldChunkWidth.Value = value;
            }
        }

        static readonly SetOnce<float> worldChunkHeight = new SetOnce<float>();
        public static float WorldChunkHeight
        {
            get
            {
                return worldChunkHeight;
            }
            set
            {
                worldChunkHeight.Value = value;
            }
        }

        public static Chunk[,] chunkMap;
    }
}
