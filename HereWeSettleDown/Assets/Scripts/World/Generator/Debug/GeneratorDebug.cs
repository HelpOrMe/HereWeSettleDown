using System.Linq;
using UnityEngine;
using World.Map;

namespace World.Generator._Debug
{
    public static class GeneratorDebuger
    {
        public static void ResetMoisture()
        {
            ClearMoisture();
            RegionsInfo.MaxMoistureIndex = int.MinValue;
            GeneratorPart.InvokePart<MoisturePart>();
            DrawMoisture();
        }

        public static void ClearMoisture()
        {
            RegionsInfo.regions.ToList().ForEach(reg => reg.type.Moisture = -1);
        }

        public static void DrawMoisture()
        {
            foreach (Region region in RegionsInfo.regions)
            {
                Color targetColor;
                if (region.type.Moisture != null)
                    targetColor = Color.Lerp(Color.black, Color.white, (float)region.type.Moisture / RegionsInfo.MaxMoistureIndex);
                else targetColor = Color.red;
                region.DoForEachPosition(pos => WorldMesh.colorMap[pos.x, pos.y].ALL = targetColor);
            }
            WorldMesh.ConfrimChangeSplitted();
        }

        public static void DrawHeight()
        {
            foreach (Region region in RegionsInfo.regions)
            {
                Color targetColor = Color.Lerp(Color.black, Color.white, (float)region.type.DistIndexFromCoastline / RegionsInfo.MaxDistIndex);
                region.DoForEachPosition(pos => WorldMesh.colorMap[pos.x, pos.y].ALL = targetColor);
            }
            WorldMesh.ConfrimChangeSplitted();
        }

        public static void DrawAllWater()
        {
            foreach (Region region in RegionsInfo.regions)
            {
                Color targetColor = region.type.isWater ? Color.white : Color.black;
                region.DoForEachPosition(pos => WorldMesh.colorMap[pos.x, pos.y].ALL = targetColor);
            }
            DrawRivers();
            WorldMesh.ConfrimChangeSplitted();
        }

        public static void DrawOcean()
        {
            foreach (Region region in RegionsInfo.regions.Where(region => region.type.isWater && !region.type.isLake))
            {
                region.DoForEachPosition(pos => WorldMesh.colorMap[pos.x, pos.y].ALL = Color.white);
            }
            WorldMesh.ConfrimChangeSplitted();
        }

        public static void DrawRivers()
        {
            foreach (River river in RiversInfo.rivers)
            {
                foreach (Vector2Int pos in river.path)
                {
                    // Draw lakes white
                    Vector2Int quadPos = WorldMesh.VertexPosToQuadPos(pos);
                    WorldMesh.colorMap[quadPos.x, quadPos.y].ALL = Color.white;
                }
            }
            WorldMesh.ConfrimChangeSplitted();
        }

        public static void DrawLakes()
        {
            foreach (Region region in RegionsInfo.regions.Where(region => region.type.isLake))
            {
                region.DoForEachPosition(pos => WorldMesh.colorMap[pos.x, pos.y].ALL = Color.white);
            }
            WorldMesh.ConfrimChangeSplitted();
        }

        public static void DrawGround()
        {
            foreach (Region region in RegionsInfo.regions.Where(region => region.type.isGround))
            {
                region.DoForEachPosition(pos => WorldMesh.colorMap[pos.x, pos.y].ALL = Color.white);
            }
            WorldMesh.ConfrimChangeSplitted();
        }
    }
}
