namespace World.Map
{
    public class ColorMap
    {
        public ColorQuad[,] map;
        public ColorQuad this[int x, int y]
        {
            get
            {
                if (IsValid(x, y))
                    return map[x, y];
                return new ColorQuad(-1, -1);
            }
            set
            {
                if (IsValid(x, y))
                {
                    map[x, y] = value;
                }
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

        public ColorMap(ColorQuad[,] map)
        {
            this.map = map;
        }

        public bool IsValid(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0 && y < height);
        }
    }
}

