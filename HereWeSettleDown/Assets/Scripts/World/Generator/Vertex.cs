using System.Collections.Generic;
using UnityEngine;

namespace World.Generator
{
    public class Vertex
    {
        public readonly Vector2Int position;
        public readonly List<Region> incidentRegions = new List<Region>();
        public readonly List<Vertex> incidentVertices = new List<Vertex>();

        public Vertex(Vector2Int position)
        {
            this.position = position;
        }

        public void ConnectTo(Vertex vert)
        {
            if (!incidentVertices.Contains(vert))
            {
                incidentVertices.Add(vert);
                vert.ConnectTo(this);
            }
        }

        public static implicit operator Vector2Int(Vertex vert) => vert.position;
        public static implicit operator Vector2(Vertex vert) => vert.position;
    }
}
