using Helper.Math;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.Generator
{
    public class Region
    {
        public readonly Vertex[] vertices;
        public readonly Site site;

        public readonly Dictionary<Vector2Int, Vector2Int> ranges = new Dictionary<Vector2Int, Vector2Int>();
        public readonly Vector2Int[] bounds;

        public readonly List<Region> neighbours = new List<Region>();

        public readonly RegionType type;

        public Region(Vector2 site, Vertex[] vertices)
        {
            type = new RegionType(this);

            this.site = new Site(this, site);
            this.vertices = vertices;

            UpdateVertices();

            bounds = MathVert.GetBoundsBetween(EdgePositions());
            ranges = MathVert.GetRangesBetween(bounds);
        }

        private void UpdateVertices()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                int b = (i - 1) == -1 ? vertices.Length - 1 : i - 1;
                int f = (i + 1) == vertices.Length ? 0 : i + 1;

                vertices[i].ConnectTo(vertices[b]);
                vertices[i].ConnectTo(vertices[f]);
                vertices[i].incidentRegions.Add(this);
            }
        }

        public Vector2[] EdgePositions()
        {
            Vector2[] edgePositions = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                edgePositions[i] = vertices[i].position;
            }

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

        public Vector2Int[] GetRegionPositions()
        {
            return MathVert.GetPositionsBetween(ranges);
        }
    }
}
