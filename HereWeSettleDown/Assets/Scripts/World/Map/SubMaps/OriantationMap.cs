namespace World.Map
{
    public class OriantationMap
    {
        public int[,] map;
        public int this[int x, int y]
        {
            get
            {
                if (IsValid(x, y))
                {
                    return map[x, y];
                }

                return 0;
            }
            set
            {
                if (IsValid(x, y))
                {
                    map[x, y] = value;
                    WorldMesh.SetEditedPosition(x, y);
                }
            }
        }

        public int width => map.GetLength(0);
        public int height => map.GetLength(1);

        public OriantationMap(int[,] map)
        {
            this.map = map;
        }

        public bool IsValid(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0 && y < height);
        }
    }
}
