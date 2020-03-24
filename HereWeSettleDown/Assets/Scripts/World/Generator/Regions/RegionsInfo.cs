using UnityEngine;
using World.Map;

namespace World.Generator
{
    public static class RegionsInfo
    {
        public static Region[] regions;
        public static Region[,] regionsMap;

        public static int MinDistIndex { get; private set; } = int.MaxValue;
        public static int MaxDistIndex { get; private set; } = int.MinValue;

        public static int MinMoistureIndex { get; private set; } = int.MaxValue;
        public static int MaxMoistureIndex { get; private set; } = int.MinValue;

        public static void UpdateDistIndex(int value)
        {
            if (value < MinDistIndex) MinDistIndex = value;
            if (value > MaxDistIndex) MaxDistIndex = value;
        }

        public static void UpdateMoistureIndex(int value)
        {
            if (value < MinMoistureIndex) MinMoistureIndex = value;
            if (value > MaxMoistureIndex) MaxMoistureIndex = value;
        }

        public static void UpdateRegionsMap()
        {
            regionsMap = new Region[WorldChunkMap.worldWidth + 1, WorldChunkMap.worldHeight + 1];
            foreach (Region region in regions)
            {
                Vector2Int[] positions = region.GetRegionPositions();
                foreach (Vector2Int pos in positions)
                {
                    regionsMap[pos.x, pos.y] = region;
                }
            }
        }
    }
}
