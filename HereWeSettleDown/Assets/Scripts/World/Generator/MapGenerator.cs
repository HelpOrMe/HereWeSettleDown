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
                SetMountains, SetLakes, DrawRegionColors,
                SmoothColors);//, ReshuffleVertices);

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
                    siteToRegion[site].AddNeighbour(siteToRegion[nSite]);
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
                Vector2Int site = new Vector2Int((int)region.site.x, (int)region.site.y);
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

        private void SetMountains()
        {
            List<Region> farRegions = new List<Region>();
            foreach (Region region in regions)
            {
                if (region.type.DistIndexFromCoastline + 1 >= maxDistIndex)
                {
                    farRegions.Add(region);
                }
            }


            List<Region> mountainRegions = farRegions;

            int layerHeight = mountainSize - 1;
            while (true)
            {
                List<Region> mRegionsClone = new List<Region>(mountainRegions);
                mountainRegions.Clear();

                foreach (Region region in mRegionsClone)
                {
                    region.type.MarkAsMountain();
                    foreach (Region nRegion in region.neighbours)
                    {
                        if (nRegion.type.HeightIndex <= layerHeight)
                        {
                            nRegion.type.HeightIndex = layerHeight;
                            mountainRegions.Add(nRegion);
                        }
                    }
                }
                if (mountainRegions.Count == 0 || layerHeight == 0)
                    break;
                layerHeight--;
            }
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
                        lakes.Add(new Lake(edge));
                        break;
                    } 
                }
            }
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
