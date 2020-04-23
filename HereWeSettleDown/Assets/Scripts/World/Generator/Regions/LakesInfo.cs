using UnityEngine;
using World.Map;

namespace World.Generator
{
    public static class LakesInfo
    {
        public static Region[] lakes;
        public static Region[,] lakesMap;

        public static void UpdateLakesMap()
        {
            lakesMap = new Region[WorldChunkMap.worldWidth + 1, WorldChunkMap.worldHeight + 1];
            foreach (Region lakeRegion in lakes)
            {
                foreach (Vector2Int pos in lakeRegion.GetRegionPositions())
                {
                    lakesMap[pos.x, pos.y] = lakeRegion;
                }
            }
        }
    }
}

