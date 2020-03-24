using UnityEngine;
using World.Map;

namespace World.Generator
{
    public static class LakesInfo
    {
        public static Lake[] lakes;
        public static Lake[,] lakesMap;

        public static void UpdateLakesMap()
        {
            lakesMap = new Lake[WorldChunkMap.worldWidth, WorldChunkMap.worldHeight];
            foreach (Lake lake in lakes)
            {
                foreach (Vector2Int pos in lake.path)
                {
                    lakesMap[pos.x, pos.y] = lake;
                }
            }
        }
    }
}

