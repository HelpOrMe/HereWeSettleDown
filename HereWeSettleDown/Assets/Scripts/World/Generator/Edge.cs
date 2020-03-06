using System.Collections.Generic;
using UnityEngine;

namespace World.Generator
{
    public class Edge
    {
        public readonly Vector2Int position;
        public readonly List<Region> incidentRegions = new List<Region>();
        public readonly List<Edge> incidentEdges = new List<Edge>();

        public Edge(Vector2Int position)
        {
            this.position = position;
        }

        public void ConnectTo(Edge edge)
        {
            if (!incidentEdges.Contains(edge))
            {
                incidentEdges.Add(edge);
                edge.ConnectTo(this);
            }
        }

        public static implicit operator Vector2Int(Edge edge) => edge.position;
        public static implicit operator Vector2(Edge edge) => edge.position;
    }
}
