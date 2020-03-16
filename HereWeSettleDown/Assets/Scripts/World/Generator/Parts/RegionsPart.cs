using System.Collections.Generic;
using UnityEngine;
using Delaunay;
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

        protected override void Run()
        {
            Watcher.WatchRun(SetVoronoi, SetRegions, CalculateTriangles);
        }

        private void SetVoronoi()
        {
            List<Vector2> points = new List<Vector2>();
            List<uint> idkWhatMeanColorsInVoronoi = new List<uint>(); 
            for (int i = 0; i < settings.cellsCount; i++)
            {
                idkWhatMeanColorsInVoronoi.Add(0);
                points.Add(new Vector2(Seed.Range(0, settings.worldWidth), Seed.Range(0, settings.worldHeight)));
            }

            voronoi = new Voronoi(points, idkWhatMeanColorsInVoronoi, new Rect(0, 0, settings.worldWidth, settings.worldHeight), Seed.prng);
        }

        private void SetRegions()
        {
            Dictionary<Vector2Int, Vertex> posToVertex = new Dictionary<Vector2Int, Vertex>();
            List<Region> regions = new List<Region>();

            foreach (Vector2 sitePos in voronoi.SiteCoords())
            {
                List<Vertex> vertices = new List<Vertex>();
                foreach (Vector2Int vertPos in MathVert.ToVector2Int(voronoi.Region(sitePos)))
                {
                    //Drawer.DrawHLine(edgePos, Color.blue);
                    if (!posToVertex.ContainsKey(vertPos))
                        posToVertex.Add(vertPos, new Vertex(vertPos));
                    vertices.Add(posToVertex[vertPos]);
                }

                Region region = new Region(sitePos, vertices.ToArray());
                siteToRegion[sitePos] = region;
                regions.Add(region);
            }

            foreach (Vector2 site in voronoi.SiteCoords())
            {
                foreach (Vector2 nSite in voronoi.NeighborSitesForSite(site))
                {
                    siteToRegion[site].neighbours.Add(siteToRegion[nSite]);
                }
            }

            RegionsPart.regions = regions.ToArray();
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
