using UnityEngine;
using World.Map;

namespace World.Generator
{
    public static class RiversInfo
    {
        public static River[] rivers;
        public static River[,] riversMap;

        public static void UpdateRiversMap()
        {
            riversMap = new River[WorldChunkMap.worldWidth + 1, WorldChunkMap.worldHeight + 1];
            foreach (River river in rivers)
            {
                foreach (Vector2Int pos in river.path)
                {
                    riversMap[pos.x, pos.y] = river;
                }
            }
        }
    }
}

