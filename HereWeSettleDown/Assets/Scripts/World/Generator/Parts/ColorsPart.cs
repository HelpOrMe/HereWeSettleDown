using UnityEngine;
using World.Map;
using Helper.Debugger;
using Settings;
using Settings.Generator;

namespace World.Generator
{
    public class ColorsPart : GeneratorPart
    {
        private ColorsSettings colorsSettings = SettingsObject.GetObject<ColorsSettings>();


        protected override void Run()
        {
            Watcher.WatchRun(DrawRegionColors, SmoothColors);
        }

        private void DrawRegionColors()
        {
            foreach (Region region in RegionsInfo.regions)
            {
                BiomeColors biomeColors = colorsSettings.biomeColors[region.type.biomeType];
                Vector2Int[] positions = region.GetRegionPositions();

                foreach (Vector2Int pos in positions)
                {
                    Color targetColor = Color.black;
                    if (region.type.isWater)
                    {
                        targetColor = biomeColors.waterColor;
                    }
                    else
                    {
                        for (int i = 0; i < colorsSettings.heightLayers.Count; i++)
                        {
                            if (WorldMesh.verticesMap[pos.x, pos.y].y <= WorldMesh.maxVertHeight * colorsSettings.heightLayers[i])
                            {
                                targetColor = biomeColors.heightColors[i];
                                break;
                            }
                        }
                    }
                    
                    Vector2Int quadPos = WorldMesh.VertexPosToQuadPos(pos);
                    WorldMesh.colorMap[quadPos.x, quadPos.y].ALL = targetColor;
                }
            }
            /*foreach (Region region in RegionsInfo.regions)
            {
                Color color = Color.white;
                if (region.type.isGround) color = Color.Lerp(Color.green, color, 0.25f);
                if (region.type.isWater) color = Color.Lerp(Color.blue, color, 0.5f);
                if (region.type.isCoastline) color = Color.Lerp(Color.yellow, color, 0.8f);
                color = Color.Lerp(color, Color.black, (float)region.type.DistIndexFromCoastline / RegionsInfo.MaxDistIndex);

                color = Color.Lerp(Color.Lerp(Color.yellow, Color.white, 0.5f), Color.Lerp(Color.green, Color.black, 0.5f), (float)region.type.Moisture / RegionsInfo.MaxDistIndex);
                if (region.type.isWater) color = Color.Lerp(Color.blue, color, 0.3f);
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
            }*/
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
