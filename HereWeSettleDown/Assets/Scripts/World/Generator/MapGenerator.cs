// Test

using System.Collections.Generic;
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
        public HeightMapGenerationGraph heightMapGraph;

        public static Voronoi voronoi;
        public static Region[] regions;

        public static Region[] coastlineRegions;
        public static Dictionary<Vector2, Region> siteToRegion = new Dictionary<Vector2, Region>();

        private float[,] waterMask;

        private System.Random prng;

        private void Awake()
        {
            prng = new System.Random(seed);
        }

        public void GenerateMap()
        {
            Watcher.Watch(() => WorldMesh.CreateWorldMesh(worldWidth, worldHeight, chunkWidth, chunkHeight), "CreateWorldMesh");
            Watcher.Watch(() => WorldChunkMap.CreateMap(worldWidth, worldHeight, chunkWidth, chunkHeight, chunkObject.transform.localScale), "CreateMap");
            Watcher.Watch(() => WorldChunkMap.CreateChunks(chunkObject, transform, true), "CreateChunks");

            AThread thread = new AThread(SetVoronoi, SetRegions, GenerateWaterMask, SetWater, SetCoastline, SetRegionHeights);
            thread.Start();
            thread.RunAfterThreadEnd(DrawRegionColors);
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
            List<Region> regions = new List<Region>();
            foreach (Vector2 site in voronoi.SiteCoords())
            {
                Vector2Int[] edges = ToVector2Int(voronoi.Region(site).ToArray());
                Region region = new Region(edges, site);
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
            heightMapGraph.mapWidth = worldWidth;
            heightMapGraph.mapHeight = worldHeight;
            heightMapGraph.setPrng = prng;

            waterMask = heightMapGraph.requester.GetHeightMap().map;
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

        private void SetRegionHeights()
        {
            foreach (Region region in regions)
            {
                float distance = 0;
                for (int i = 0; i < coastlineRegions.Length; i++)
                    distance += Vector2.Distance(region.site, coastlineRegions[i].site);
                //Debug.Log(distance / coastlineRegions.Length);
            }
        }

        private void DrawRegionColors()
        {
            foreach (Region region in regions)
            {
                Color color = Color.white;
                if (region.type.isGround) color = Color.Lerp(Color.green, color, 0.35f);
                if (region.type.isWater) color = Color.Lerp(Color.blue, color, 0.5f);
                if (region.type.isCoastline) color = Color.Lerp(Color.yellow, color, 0.8f);

                region.DoForEachPosition((Vector2Int point) => WorldMesh.colorMap[point.x, point.y].ALL = color);
            }
            WorldMesh.ConfirmChanges();
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
