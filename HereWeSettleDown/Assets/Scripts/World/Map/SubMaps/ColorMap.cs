using UnityEngine;

namespace World.Map
{
    public class ColorMap
    {
        public ColorPack[,] map;
        public Color this[int x, int y, int i]
        {
            get
            {
                if (IsValid(x, y))
                    return map[x, y][i];
                return Color.black;
            }
            set
            {
                if (IsValid(x, y))
                {
                    map[x, y][i] = value;
                    WorldMeshMap.SetEditedPosition(x, y);
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

        public ColorMap(ColorPack[,] map)
        {
            this.map = map;
        }

        public bool IsValid(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0 && y < height);
        }
    }
}

