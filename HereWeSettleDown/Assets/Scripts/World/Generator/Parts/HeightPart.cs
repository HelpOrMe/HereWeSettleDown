using Helper.Debugger;
using Helper.Math;
using Settings;
using Settings.Generator;
using UnityEngine;
using World.Map;

namespace World.Generator
{
    public class HeightPart : GeneratorPart
    {
        private readonly HeightSettings heightSettings = SettingsObject.GetObject<HeightSettings>();

        protected override void Run()
        {
            Watcher.WatchRun(SetMapHeight, SmoothHeight, SetHeightCurve);
        }

        private void SetMapHeight()
        {
            foreach (Triangle triangle in Triangle.allTriangles)
            {
                if (triangle.GetMidCLIndex() <= 0)
                    continue;

                //Drawer.DrawConnectedLines(triangle.GetSitePositions(), Color.white);

                Vector3[] trianglePoints = new Vector3[3];
                for (int i = 0; i < 3; i++)
                    trianglePoints[i] = SiteWithHeight(triangle.sites[i].parent);

                Vector2Int[] bounds = MathVert.GetBoundsBetween(triangle.GetSitePositions());
                for (int i = 0; i < bounds.Length - 1; i++)
                {
                    Vector2Int quadPos = WorldMesh.VertexPosToQuadPos(bounds[i], bounds[i + 1]);
                    int oriantation = WorldMesh.GetOriantation(bounds[i], bounds[i + 1]);
                    // WorldMesh.oriantationMap[quadPos.x, quadPos.y] = oriantation;
                }

                Vector2Int[] positions = MathVert.GetPositionsBetween(MathVert.GetRangesBetween(bounds));
                foreach (Vector2Int position in positions)
                {
                    //Drawer.DrawHLine(position, Color.white);
                    //WorldMesh.colorMap[position.x, position.y].ALL = Color.red;
                    Vector3 vertex = WorldMesh.verticesMap[position.x, position.y];
                    vertex.y = MathVert.GetHeihtBetween3Points(position, trianglePoints);
                    WorldMesh.verticesMap[position.x, position.y] = vertex;
                }
            }
        }

        private Vector3 SiteWithHeight(Region region)
        {
            if (region.type.DistIndexFromCoastline <= 0)
                return MathVert.ToVector3(region.site);

            float dist = (float)region.type.DistIndexFromCoastline;
            dist = Mathf.Clamp(dist - heightSettings.heightOffset, 1, dist);

            float height = Mathf.Round(Mathf.Pow(dist / RegionsInfo.MaxDistIndex * heightSettings.heightValue, 2));
            return MathVert.ToVector3(region.site) + Vector3.up * height;
        }

        private void SmoothHeight()
        {
            Vector2Int[] offsets = new Vector2Int[]
            {
                Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
            };

            for (int i = 0; i < heightSettings.heightSmoothIterations; i++)
            {
                for (int x = 1; x < WorldMesh.verticesMap.width - 1; x++)
                {
                    for (int y = 1; y < WorldMesh.verticesMap.height - 1; y++)
                    {
                        Vector3 midPoint = Vector3.zero;
                        foreach (Vector2Int offset in offsets)
                            midPoint += WorldMesh.verticesMap[x + offset.x, y + offset.y];
                        Vector3 vert = Vector3.Scale(midPoint, new Vector3(0.25f, 0.25f, 0.25f));
                        WorldMesh.verticesMap[x, y] = Vector3.Lerp(WorldMesh.verticesMap[x, y], vert, heightSettings.heightSmoothCoef);
                    }
                }
            }
        }

        private void SetHeightCurve()
        {
            for (int x = 0; x < WorldMesh.verticesMap.width; x++)
            {
                for (int y = 0; y < WorldMesh.verticesMap.height; y++)
                {
                    Vector3 vert = WorldMesh.verticesMap[x, y];
                    vert.y = heightSettings.heightCurve.Evaluate(vert.y / WorldMesh.maxVertHeight) * WorldMesh.maxVertHeight;
                    WorldMesh.verticesMap[x, y] = vert;
                }
            }
        }
    }
}
