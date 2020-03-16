using UnityEngine;
using World.Map;
using Helper.Debugger;

namespace World.Generator
{
    public class ColorsPart : GeneratorPart
    {
        public override void Run()
        {
            Watcher.WatchRun(DrawRegionColors, SmoothColors);
        }

        private void DrawRegionColors()
        {
            foreach (Region region in RegionsPart.regions)
            {
                Color color = Color.white;
                if (region.type.isGround) color = Color.Lerp(Color.green, color, 0.25f);
                if (region.type.isWater) color = Color.Lerp(Color.blue, color, 0.5f);
                if (region.type.isCoastline) color = Color.Lerp(Color.yellow, color, 0.8f);
                color = Color.Lerp(color, Color.black, (float)region.type.DistIndexFromCoastline / RegionType.MaxDistIndex);

                //color = Color.Lerp(Color.Lerp(Color.yellow, Color.white, 0.5f), Color.Lerp(Color.green, Color.black, 0.5f), (float)region.type.Wet / maxDistIndex);
                //if (region.type.isWater) color = Color.Lerp(Color.blue, Color.white, 0.3f);

                region.DoForEachPosition((Vector2Int point) => WorldMesh.colorMap[point.x, point.y].ALL = color);
            }

            foreach (Lake lake in LakesPart.lakes)
            {
                foreach (Vector2Int pathPoint in lake.path)
                {
                    Vector2Int point = WorldMesh.VertexPosToQuadPos(pathPoint);
                    WorldMesh.colorMap[point.x, point.y].ALL = Color.blue;
                }
            }
        }

        private void SmoothColors()
        {
            for (int x = 0; x < WorldMesh.colorMap.width; x++)
            {
                for (int y = 0; y < WorldMesh.colorMap.height; y++)
                {
                    WorldMesh.colorMap[x, y].Smooth();
                }
            }
        }
    }
}
