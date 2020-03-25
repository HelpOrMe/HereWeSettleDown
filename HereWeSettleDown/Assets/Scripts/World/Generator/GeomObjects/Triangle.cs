using System.Collections.Generic;
using UnityEngine;

namespace World.Generator
{
    public class Triangle
    {
        public static List<Triangle> allTriangles = new List<Triangle>();
        public readonly Site[] sites;

        public Triangle(Site site1, Site site2, Site site3)
        {
            sites = new Site[3];
            sites[0] = site1;
            sites[1] = site2;
            sites[2] = site3;
        }

        public void Confirm()
        {
            AddInSites();
            allTriangles.Add(this);
        }

        public void AddInSites()
        {
            foreach (Site site in sites)
            {
                site.containsIn.Add(this);
            }
        }

        public Vector2[] GetSitePositions()
        {
            Vector2[] sitePositions = new Vector2[3];
            for (int i = 0; i < 3; i++)
            {
                sitePositions[i] = sites[i];
            }

            return sitePositions;
        }

        public float GetMidCLIndex()
        {
            float midInd = 0;
            foreach (Site site in sites)
            {
                midInd += (float)site.parent.type.DistIndexFromCoastline;
            }
            return midInd / 3;
        }
    }
}
