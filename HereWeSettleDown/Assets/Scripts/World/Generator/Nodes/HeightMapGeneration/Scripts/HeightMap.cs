namespace World.Generator.Nodes.HeightMap
{
    [System.Serializable]
    public class HeightMap
    {
        public float[,] map;
        public float this[int x, int y]
        {
            get => map[x, y];
            set => map[x, y] = value;
        }

        public int width => map.GetLength(0);

        public int height => map.GetLength(1);

        public HeightMap(float[,] map)
        {
            this.map = map;
        }
    }
}

