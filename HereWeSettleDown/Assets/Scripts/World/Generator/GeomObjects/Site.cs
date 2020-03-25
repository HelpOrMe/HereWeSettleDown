using System.Collections.Generic;
using UnityEngine;

namespace World.Generator
{
    public class Site
    {

        public readonly Region parent;
        public readonly Vector2 position;
        public readonly List<Triangle> containsIn = new List<Triangle>();

        public Site(Region parent, Vector2 position)
        {
            this.parent = parent;
            this.position = position;
        }

        public void CalculateTriangles()
        {
            Site[] sites = GetNeighbourSites();

            if (sites.Length > 0)
            {
                List<Triangle> triangles = new List<Triangle>();
                for (int i = 0; i < sites.Length - 1; i++)
                {
                    triangles.Add(new Triangle(sites[i], sites[i + 1], this));
                }
                triangles.Add(new Triangle(sites[sites.Length - 1], sites[0], this));

                foreach (Triangle triangle in triangles)
                {
                    if (!containsIn.Contains(triangle))
                    {
                        triangle.Confirm();
                    }
                }
            }
        }

        public Site[] GetNeighbourSites()
        {
            List<Site> neighbourSites = new List<Site>();
            foreach (Region region in parent.neighbours)
            {
                neighbourSites.Add(region.site);
            }
            return neighbourSites.ToArray();
        }

        public static implicit operator Vector2(Site site)
        {
            return site.position;
        }
    }
}

