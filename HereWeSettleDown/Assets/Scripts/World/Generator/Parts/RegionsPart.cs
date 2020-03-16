using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using Helper.Math;
using Helper.Random;
using Helper.Debugger;

namespace World.Generator
{
    public class RegionsPart : GeneratorPart
    {
        public static Voronoi voronoi;
        public static Region[] regions;
        public static Dictionary<Vector2, Region> siteToRegion = new Dictionary<Vector2, Region>();

        public override void Run()
        {
            Watcher.WatchRun(SetVoronoi, SetRegions, CalculateTriangles);
        }

        private void SetVoronoi()
        {
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < settings.cellsCount; i++)
            {
                points.Add(new Vector2(Seed.Range(0, settings.worldWidth), Seed.Range(0, settings.worldHeight)));
            }
            voronoi = new Voronoi(points, new Rectf(0, 0, settings.worldWidth, settings.worldHeight), 5, Seed.prng);
        }

        private void SetRegions()
        {
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
        }

        private void CalculateTriangles()
        {
            foreach (Region region in regions)
            {
                region.site.CalculateTriangles();
            }
        }
    }
}
