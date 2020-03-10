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

        public int mountainSize = 5;
        public Vector2Int lakesCount;

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
                SetLakes, DrawRegionColors, SmoothColors,
                CalculateTriangles, SetMapHeight);//, ReshuffleVertices);

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
            voronoi = new Voronoi(points, new Rectf(0, 0, worldWidth, worldHeight), 2, prng);
        }

        private void SetRegions()
        {
            Dictionary<Vector2Int, Edge> posToEdge = new Dictionary<Vector2Int, Edge>();

            List<Region> regions = new List<Region>();
            foreach (Vector2 site in voronoi.SiteCoords())
            {
                List<Edge> edges = new List<Edge>();
                foreach (Vector2Int edgePos in ToVector2Int(voronoi.Region(site).ToArray()))
                {
                    if (!posToEdge.ContainsKey(edgePos))
                        posToEdge[edgePos] = new Edge(edgePos);
                    edges.Add(posToEdge[edgePos]);
                }

                Region region = new Region(edges.ToArray(), site);
                siteToRegion[site] = region;
                regions.Add(region);
            }

            foreach (Vector2 site in voronoi.SiteCoords())
            {
                foreach (Vector2 nSite in voronoi.NeighborSitesForSite(site))
                {
                    siteToRegion[site].neighbours.Add(siteToRegion[nSite]);
                }
            }

            MapGenerator.regions = regions.ToArray();
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

            while (true)
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

                if (regionsLayer.Count == 0)
                    break;
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
                        Edge edge = region.edges[prng.Next(0, region.edges.Length)];
                        Lake lake = new Lake(edge);
                        lake.Set();
                        lakes.Add(lake);
                        break;
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
            /*foreach (Region region in regions)
            {
                foreach (Region nRegion in region.neighbours)
                {
                    Vector3 point1 = SiteWithHeight(region);
                    Vector3 point2 = SiteWithHeight(nRegion);

                    //Drawer.DrawLine(MathVert.RoundPosition(point1), MathVert.RoundPosition(point2), Color.red);

                    Vector3[] path = MathVert.ConnectPoints(point1, point2, false);
                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        //Drawer.DrawLine(path[i], path[i + 1], Color.white);
                    }
                }
            }*/
            
            foreach (Triangle triangle in Triangle.allTriangles)
            {
                if (triangle.GetMidCLIndex() <= 0)
                    continue;

                //Drawer.DrawConnectedLines(triangle.GetSitePositions(), Color.white);

                Vector3[] trianglePoints = new Vector3[3];
                for (int i = 0; i < 3; i++)
                    trianglePoints[i] = SiteWithHeight(triangle.sites[i].parent);

                Vector2Int[] positions = MathVert.GetPositionsBetween(triangle.GetSitePositions());
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

            int offset = 4;

            float dist = (float)region.type.DistIndexFromCoastline;
            dist = Mathf.Clamp(dist - offset, 1, dist);

            float height = Mathf.Pow(dist / maxDistIndex * 10, 2);
            return MathVert.ToVector3(region.site) + Vector3.up * height;
        }

        

        private void DrawRegionColors()
        {
            foreach (Region region in regions)
            {
                Color color = Color.white;
                if (region.type.isGround) color = Color.Lerp(Color.green, color, 0.25f);
                if (region.type.isWater) color = Color.Lerp(Color.blue, color, 0.5f);
                if (region.type.isCoastline) color = Color.Lerp(Color.yellow, color, 0.8f);
                color = Color.Lerp(color, Color.black, (float)region.type.DistIndexFromCoastline / maxDistIndex);
                //color = Color.Lerp(color, Color.white, (float)region.type.HeightIndex / mountainSize);

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
