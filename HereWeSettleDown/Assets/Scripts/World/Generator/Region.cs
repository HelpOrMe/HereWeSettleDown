using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Helper.Math;

namespace World.Generator
{
    public class Region
    {   
        public readonly Edge[] edges;
        public readonly Site site;

        public readonly Dictionary<Vector2Int, Vector2Int> ranges = new Dictionary<Vector2Int, Vector2Int>();
        public readonly Vector2Int[] bounds;

        public readonly List<Region> neighbours = new List<Region>();

        public readonly RegionType type;

        public Region(Edge[] edges, Vector2 site)
        {
            type = new RegionType(this);

            this.site = new Site(this, site);
            this.edges = edges;

            UpdateEdges();
            bounds = MathVert.GetBoundsBetween(EdgePositions());
            ranges = MathVert.GetRangesBetween(bounds);
        }

        public void UpdateEdges()
        {
            for (int i = 0; i < edges.Length; i++)
            {
                int b = (i - 1) == -1 ? edges.Length - 1 : i - 1;
                int f = (i + 1) == edges.Length ? 0 : i + 1;
                
                edges[i].ConnectTo(edges[b]);
                edges[i].ConnectTo(edges[f]);
                edges[i].incidentRegions.Add(this);
            }
        }

        public Vector2[] EdgePositions()
        {
            Vector2[] edgePositions = new Vector2[edges.Length];
            for (int i = 0; i < edges.Length; i++)
                edgePositions[i] = edges[i].position;
            return edgePositions;
        }

        public void AddNeighbour(Region region)
        {
            if (!neighbours.Contains(region))
            {
                neighbours.Add(region);
                region.AddNeighbour(this);
            }
        }

        public void DoForEachPosition(Action<Vector2Int> action)
        {
            foreach (Vector2Int point in MathVert.GetPositionsBetween(ranges))
            {
                action.Invoke(point);
            }
        }
    }

    public class RegionType
    {
        public readonly Region parent;

        public bool isWater { get; private set; }
        public bool isGround { get; private set; }
        public bool isCoastline { get; private set; }
        public bool isMountain { get; private set; }

        public int? DistIndexFromCoastline;

        public RegionType(Region region) => parent = region;

        public void MarkAsWater()
        {
            isWater = true;
            isGround = false;
            DistIndexFromCoastline = -1;
        }

        public void MarkAsGround()
        {
            isGround = true;
            isWater = false;
        }
        
        public void MarkAsCoastline()
        {
            MarkAsWater();
            isCoastline = true;
            DistIndexFromCoastline = 0;
        }

        public void MarkAsMountain()
        {
            MarkAsGround();
            isMountain = true;
        }
    }
}
 