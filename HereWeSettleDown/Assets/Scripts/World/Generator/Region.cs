using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World.Generator
{
    public class Region
    {   
        public readonly Vector2Int[] edges;
        public readonly Vector2 site;

        public readonly Dictionary<Vector2Int, Vector2Int> ranges = new Dictionary<Vector2Int, Vector2Int>();
        public readonly List<Vector2Int> bounds = new List<Vector2Int>();

        public readonly List<Region> neighbours = new List<Region>();

        public readonly RegionType type;

        public Region(Vector2Int[] edges, Vector2 site)
        {
            type = new RegionType(this);

            this.site = site;
            this.edges = edges;

            RecalculatBounds();
            RecalculateRanges();
        }

        public void RecalculatBounds()
        {
            bounds.Clear();
            for (int i = 0; i < edges.Length; i++)
            {
                int j = (i + 1) % edges.Length;
                Vector2Int[] connectedPoints = ConnectPointsByVertices(edges[i], edges[j]);
                bounds.AddRange(connectedPoints);
            }
        }

        private Vector2Int[] ConnectPointsByVertices(Vector2 point1, Vector2 point2)
        {
            List<Vector2Int> points = new List<Vector2Int>();

            Vector2 offset = RoundPosition(point1 - point2);
            for (float i = offset.magnitude; i > 0; i--)
            {
                Vector2 point = RoundPosition(Vector2.MoveTowards(point1, point2, i));
                Vector2Int intPoint = ToVector2Int(point);

                if (!points.Contains(intPoint))
                    points.Add(intPoint);
            }
            points.Add(ToVector2Int(RoundPosition(point1)));
            return points.ToArray();
        }

        private Vector2Int ToVector2Int(Vector2 p)
        {
            return new Vector2Int((int)p.x, (int)p.y);
        }

        private Vector2 RoundPosition(Vector2 p)
        {
            return new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
        }
        
        public void RecalculateRanges()
        {
            ranges.Clear();

            Dictionary<int, List<int>> allPointsByY = new Dictionary<int, List<int>>();
            foreach (Vector2Int point in bounds)
            {
                if (!allPointsByY.ContainsKey(point.y))
                    allPointsByY.Add(point.y, new List<int>());
                allPointsByY[point.y].Add(point.x);
            }

            foreach (int yLine in allPointsByY.Keys)
            {
                Vector2Int minPos = new Vector2Int(allPointsByY[yLine].Min(), yLine);
                Vector2Int maxPos = new Vector2Int(allPointsByY[yLine].Max(), yLine);

                if (!ranges.ContainsKey(minPos))
                    ranges.Add(minPos, maxPos);
            }
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
            if (ranges.Count <= 0)
                RecalculateRanges();

            foreach (Vector2Int leftPoint in ranges.Keys)
            {
                for (int x = leftPoint.x; x <= ranges[leftPoint].x; x++)
                {
                    action.Invoke(new Vector2Int(x, leftPoint.y));
                }
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
 