// Test

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Map;
using Nodes.HeightMapGeneration;
using csDelaunay;
using Helper.Threading;
using Helper.Debugger;
using Helper.Math;

namespace World.Generator
{
    public class MapGenerator : MonoBehaviour
    {
        public int seed = 0;
        public int cellsCount = 124;

        public int worldWidth = 124;
        public int worldHeight = 124;

        public int chunkWidth = 124 / 2;
        public int chunkHeight = 124 / 2;

        public ChunkObject chunkObject;
        public HeightMapGenerationGraph waterMaskGraph;
        //public AnimationCurve mountainsCurve;

        public int mountainOffset = 3;
        public Vector2Int lakesCount;
        public int heightSmoothIterations = 2;
        public float smoothCoef = 0.4f;

        public static Voronoi voronoi;
        public static Region[] regions;

        public static Region[] coastlineRegions;
        public static Dictionary<Vector2, Region> siteToRegion = new Dictionary<Vector2, Region>();
        public static List<Lake> lakes = new List<Lake>();

        private float[,] waterMask;
        private int maxDistIndex;

        private System.Random prng;

        private void Awake()
        {
            prng = new System.Random(seed);
        }

        public void GenerateMap()
        {
            Debug.ClearDeveloperConsole();
            Watcher.Watch(() => WorldMesh.CreateWorldMesh(worldWidth, worldHeight, chunkWidth, chunkHeight), "CreateWorldMesh");
            Watcher.Watch(() => WorldChunkMap.CreateMap(worldWidth, worldHeight, chunkWidth, chunkHeight, chunkObject.transform.localScale), "CreateMap");
            Watcher.Watch(() => WorldChunkMap.CreateChunks(chunkObject, transform, true), "CreateChunks");

            AThread thread = new AThread(
                SetVoronoi, SetRegions, GenerateWaterMask,
                SetWater, SetCoastline, SetRegionDistances,
                SetLakes, SetWet, CalculateTriangles, SetMapHeight, SmoothHeight,
                DrawRegionColors, SmoothColors);//, ReshuffleVertices);


            thread.Start();
            thread.RunAfterThreadEnd(() => Watcher.Watch(WorldMesh.ConfirmChanges));
        }

        private void SetVoronoi()
        {
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < cellsCount; i++)
            {
                points.Add(new Vector2(prng.Next(0, worldWidth), prng.Next(0, worldHeight)));
            }
            voronoi = new Voronoi(points, new Rectf(0, 0, worldWidth, worldHeight), 5, prng);

            /*foreach (Vector2 site in voronoi.SiteCoords())
            {
                List<Vector2> region = voronoi.Region(site);
                if (region == null || region.Count <= 1) continue; 

                for (int i = 0; i < region.Count - 1; i++)
                    Drawer.DrawLine(Vector2.Lerp(site, region[i], 0.9f), Vector2.Lerp(site, region[i + 1], 0.9f), Color.blue);
                Drawer.DrawLine(Vector2.Lerp(site, region[0], 0.9f), Vector2.Lerp(site, region[region.Count - 1], 0.9f), Color.blue);
            }*/
        }

        private void SetRegions()
        {

            /*var posToVertex = new Dictionary<Vector2, Vertex>();
            foreach (var edge in voronoi.Edges)
            {
                if (edge.Visible())
                {
                    Vector2[] vertices = new Vector2[] { edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT] };
                    foreach (Vector2 vert in vertices)
                    {
                        if (!posToVertex.ContainsKey(vert))
                        {
                            posToVertex.Add(vert, new Vertex(MathVert.ToVector2Int(vert)));
                        }
                    }
                }
            }*/

            /*var siteToEdges = new Dictionary<csDelaunay.Site, List<Edge>>();
            var siteNeigbours = new Dictionary<csDelaunay.Site, List<csDelaunay.Site>>();

            foreach (var edge in voronoi.Edges)
            {
                if (edge.Visible())
                {
                    var sites = new csDelaunay.Site[] { edge.RightSite, edge.LeftSite };
                    foreach (var site in sites)
                    {
                        if (!siteNeigbours.ContainsKey(site))
                            siteNeigbours.Add(site, new List<csDelaunay.Site>());

                        if (!siteToEdges.ContainsKey(site))
                            siteToEdges.Add(site, new List<Edge>());
                        siteToEdges[site].Add(edge);

                        /*Vector2[] vertices = new Vector2[] { edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT] };
                        foreach (Vector2 vert in vertices)
                        {
                            if (!siteToVertices[site].Contains(posToVertex[vert]))
                            {
                                siteToVertices[site].Add(posToVertex[vert]);
                            }
                        }
                    }

                    siteNeigbours[edge.RightSite].Add(edge.LeftSite);
                    siteNeigbours[edge.LeftSite].Add(edge.RightSite);
                }
            }

            List<Region> regions = new List<Region>();
            var siteToRegion = new Dictionary<csDelaunay.Site, Region>();

            foreach (var site in siteToEdges.Keys)
            {
                Region region = new Region(site.Coord, siteToEdges[site]);
                regions.Add(region);
                siteToRegion[site] = region;
            }

            foreach (var site in siteNeigbours.Keys)
            {
                Region region = siteToRegion[site];
                foreach (var nSite in siteNeigbours[site])
                {
                    region.AddNeighbour(siteToRegion[nSite]);
                }
            }

            MapGenerator.regions = regions.ToArray();

            foreach (Region region in regions)
            {
                Drawer.DrawConnectedLines(region.bounds, Color.white);
            }

            foreach (var edge in voronoi.Edges)
            {
                if (edge.Visible())
                    Drawer.DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], Color.white);
            }*/

            

            Dictionary<Vector2Int, Vertex> posToVertex = new Dictionary<Vector2Int, Vertex>();
            List<Region> regions = new List<Region>();

            foreach (var site in voronoi.SitesIndexedByLocation.Values)
            {
                List<Vertex> vertices = new List<Vertex>();
                foreach (Vector2Int vertPos in MathVert.ToVector2Int(voronoi.Region(site.Coord)))
                {
                    //Drawer.DrawHLine(edgePos, Color.blue);
                    if (!posToVertex.ContainsKey(vertPos))
                        posToVertex.Add(vertPos, new Vertex(vertPos));
                    vertices.Add(posToVertex[vertPos]);
                }

                Region region = new Region(site.Coord, vertices.ToArray());
                siteToRegion[site.Coord] = region;
                regions.Add(region);
            }

            foreach (Vector2 site in voronoi.SitesIndexedByLocation.Keys)
            {
                foreach (Vector2 nSite in voronoi.NeighborSitesForSite(site))
                {
                    siteToRegion[site].neighbours.Add(siteToRegion[nSite]);
                }
            }

            MapGenerator.regions = regions.ToArray();

            foreach (Region region in regions)
            {
                Drawer.DrawConnectedLines(region.bounds, Color.red);
            }
        }

        private void GenerateWaterMask()
        {
            waterMaskGraph.mapWidth = worldWidth;
            waterMaskGraph.mapHeight = worldHeight;
            waterMaskGraph.setPrng = prng;
            waterMask = waterMaskGraph.requester.GetHeightMap().map;
        }

        private void SetWater()
        {
            foreach (Region region in regions)
            {
                Vector2Int site = MathVert.ToVector2Int(region.site);
                if (waterMask[site.x, site.y] < 1) 
                    region.type.MarkAsWater();
                else 
                    region.type.MarkAsGround();
            }
        }

        private void SetCoastline()
        {
            List<Region> coastlineRegions = new List<Region>();
            foreach (Region region in regions)
            {
                foreach (Region regionNeighbour in region.neighbours)
                {
                    if (region.type.isWater && regionNeighbour.type.isGround)
                    {
                        region.type.MarkAsCoastline();
                        coastlineRegions.Add(region);
                        break;
                    }
                }
            }
            MapGenerator.coastlineRegions = coastlineRegions.ToArray();
        }

        private void SetRegionDistances()
        {
            List<Region> regionsLayer = new List<Region>(coastlineRegions);
            int layerDist = 0;

            while (regionsLayer.Count > 0)
            {
                layerDist++;

                List<Region> regionsLayerClone = new List<Region>(regionsLayer);
                regionsLayer.Clear();

                foreach (Region region in regionsLayerClone)
                {
                    foreach (Region nRegion in region.neighbours)
                    {
                        if (nRegion.type.DistIndexFromCoastline == null)
                        {
                            nRegion.type.DistIndexFromCoastline = layerDist;
                            regionsLayer.Add(nRegion);
                        }
                    }
                }
            }
            maxDistIndex = layerDist;
        }

        private void SetLakes()
        {
            int rndLakedCount = prng.Next(lakesCount.x, lakesCount.y);
            for (int i = 0; i < rndLakedCount; i++)
            {
                while (true)
                {
                    Region region = regions[prng.Next(0, regions.Length)];
                    if (region.type.isGround && region.type.DistIndexFromCoastline > 2)
                    {
                        Vertex vertex = region.vertices[prng.Next(0, region.vertices.Length)];
                        Lake lake = new Lake(vertex);
                        lake.Set();
                        lakes.Add(lake);
                        break;
                    }
                }
            }
        }
        
        private void SetWet()
        {
            foreach (Region region in regions)
            {
                region.type.Wet = maxDistIndex - region.type.DistIndexFromCoastline;
            }

            foreach (Lake lake in lakes)
            {
                List<Region> lakeRegions = new List<Region>();
                foreach (Vertex vertex in lake.vertices)
                    lakeRegions.AddRange(vertex.incidentRegions);
                SetRecWet(lakeRegions);
            }
        }

        private void SetRecWet(List<Region> regionsLayer)
        {
            int wet = maxDistIndex - 1;

            foreach (Region region in regionsLayer)
                if (region.type == null || region.type.Wet < wet)
                    region.type.Wet = wet;

            while (regionsLayer.Count > 0)
            {
                wet--;

                List<Region> oldRegionsLayer = new List<Region>(regionsLayer);
                regionsLayer.Clear();
                foreach (Region region in oldRegionsLayer)
                {
                    foreach (Region nRegion in region.neighbours)
                    {
                        if (nRegion.type == null || nRegion.type.Wet < wet)
                        {
                            nRegion.type.Wet = wet;
                            regionsLayer.Add(nRegion);
                        }
                    }
                }
            }
        }

        private void CalculateTriangles()
        {
            foreach (Region region in regions)
            {
                region.site.CalculateTriangles();
            }
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

                /*for (int i = 0; i < 2; i ++)
                {
                    Vector3[] points = MathVert.ConnectPoints(trianglePoints[i], trianglePoints[i + 1], false);
                    for (int j = 0; j < points.Length - 1; j++)
                    {
                        Vector3 point1 = points[j] + Vector3.up * 0.1f;
                        Vector3 point2 = points[j + 1];
                        Drawer.DefDrawLine(point1, point2, Color.white, float.PositiveInfinity);

                        Vector2Int quadPos = WorldMesh.VertexPosToQuadPos(MathVert.ToVector2Int(point1), MathVert.ToVector2Int(point2));
                        Vector3 midPoint = Vector3.Lerp(point1, point2, 0.5f);
                        Drawer.DefDrawLine(midPoint, new Vector3(quadPos.x, midPoint.y, quadPos.y), Color.white, float.PositiveInfinity);
                    }
                }*/

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
            dist = Mathf.Clamp(dist - mountainOffset, 1, dist);

            float height = Mathf.Pow(dist / maxDistIndex * 10, 2);
            return MathVert.ToVector3(region.site) + Vector3.up * height;
        }

        private void SmoothHeight()
        {
            Vector2Int[] offsets = new Vector2Int[]
            {
                Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
            };
            
            for (int i = 0; i < heightSmoothIterations; i++)
            {
                for (int x = 1; x < WorldMesh.verticesMap.width - 1; x++)
                {
                    for (int y = 1; y < WorldMesh.verticesMap.height - 1; y++)
                    {
                        Vector3 midPoint = Vector3.zero;
                        foreach (Vector2Int offset in offsets)
                            midPoint += WorldMesh.verticesMap[x + offset.x, y + offset.y];
                        Vector3 vert = Vector3.Scale(midPoint, new Vector3(0.25f, 0.25f, 0.25f));
                        WorldMesh.verticesMap[x, y] = Vector3.Lerp(WorldMesh.verticesMap[x, y], vert, smoothCoef);

                        /*float midHeight = 0;
                        foreach (Vector2Int offset in offsets)
                            midHeight += WorldMesh.verticesMap[x + offset.x, y + offset.y].y;
                        Vector3 vert = WorldMesh.verticesMap[x, y];
                        vert.y = midHeight / 4;
                        WorldMesh.verticesMap[x, y] = vert;*/
                    }
                }
            }

            /*
             * int dst = 2;

            for (int i = 0; i < heightSmoothIterations; i++)
            {
                foreach (Vector2Int point in points)
                {
                    for (int x = -dst; x < dst; x++)
                    {
                        for (int y = -dst; y < dst; y++)
                        {
                            int aX = point.x + x;
                            int aY = point.y + y;

                            if (!WorldMesh.verticesMap.IsValid(aX, aY))
                                continue;

                            float midHeight = 0;
                            foreach (Vector2Int offset in offsets)
                                //if (WorldMesh.verticesMap.IsValid(aX + offset.x, aY + offset.y))
                                    midHeight += WorldMesh.verticesMap[aX + offset.x, aY + offset.y].y;

                            Vector3 vert = WorldMesh.verticesMap[aX, aY];
                            vert.y = midHeight / 4;
                            WorldMesh.verticesMap[aX, aY] = vert;
                        }
                    }
                }
            }*/
        }

        private void DrawRegionColors()
        {
            /*for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    //if (WorldMesh.colorMap[x, y].LEFT != Color.red) 
                        WorldMesh.colorMap[x, y].ALL = Color.Lerp(Color.black, Color.white, WorldMesh.verticesMap[x, y].y / 100);
                }
            }*/

            foreach (Region region in regions)
            {
                Color color = Color.white;
                /*if (region.type.isGround) color = Color.Lerp(Color.green, color, 0.25f);
                if (region.type.isWater) color = Color.Lerp(Color.blue, color, 0.5f);
                if (region.type.isCoastline) color = Color.Lerp(Color.yellow, color, 0.8f);
                color = Color.Lerp(color, Color.black, (float)region.type.DistIndexFromCoastline / maxDistIndex);*/

                color = Color.Lerp(Color.Lerp(Color.yellow, Color.white, 0.5f), Color.Lerp(Color.green, Color.black, 0.5f), (float)region.type.Wet / maxDistIndex);
                if (region.type.isWater) color = Color.Lerp(Color.blue, Color.white, 0.3f);

                region.DoForEachPosition((Vector2Int point) => WorldMesh.colorMap[point.x, point.y].ALL = color);
            }

           foreach (Lake lake in lakes)
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

        private void ReshuffleVertices()
        {
            VerticesMap vertMap = WorldMesh.verticesMap;
            for (int x = 0; x < vertMap.width; x++)
            {
                for (int y = 0; y < vertMap.height; y++)
                {
                    vertMap[x, y] += new Vector3((float)prng.Next(40, 120) / 100, 0, (float)prng.Next(40, 80) / 120);
                }
            }
        }

        private Vector2Int[] ToVector2Int(Vector2[] ps)
        {
            Vector2Int[] points = new Vector2Int[ps.Length];
            for (int i = 0; i < ps.Length; i++)
                points[i] = new Vector2Int((int)ps[i].x, (int)ps[i].y);
            return points;
        }
    }
}
