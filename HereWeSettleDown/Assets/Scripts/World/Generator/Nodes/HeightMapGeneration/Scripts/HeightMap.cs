namespace World.Generator.Nodes.HeightMap
{
    [System.Serializable]
    public class HeightMap
    {
        public float[,] map;
        public float this[int x, int y]
        {
            get
            {
                return map[x, y];
            }
            set
            {
                map[x, y] = value;
            }
        }

        public int width
        {
            get
            {
                return map.GetLength(0);
            }
        }

        public int height
        {
            get
            {
                return map.GetLength(1);
            }
        }

        public HeightMap(float[,] map)
        {
            this.map = map;
        }
    }
}

