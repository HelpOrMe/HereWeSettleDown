using Helper.Math;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using Helper.Debugger;

namespace World.Generator
{
    public class River
    {
        public readonly Vertex startVertex;
        public readonly List<Vertex> vertices = new List<Vertex>();
        public readonly List<Vector2Int> path = new List<Vector2Int>();

        public River(Vertex startVertex)
        {
            this.startVertex = startVertex;
        }

        public void Set()
        {
            CalculateEdges();
            CalculatePath();
            SetPathWidth();
        }

        private void CalculateEdges()
        {
            //Drawer.DrawHLine(startEdge, Color.blue);
            vertices.Clear();
            Vertex lastVertex = startVertex;
            for (int i = 0; i < 100; i++) // while
            {
                Vertex newVertex = FindLowestEdgeNear(lastVertex);
                if (newVertex != null)
                {
                    //Drawer.DrawHLine(newEdge, Color.blue);
                    //Drawer.DrawLine(lastEdge, newEdge, Color.blue);
                    vertices.Add(newVertex);
                    lastVertex = newVertex;
                    if (GetVertexCoastlineDist(newVertex) <= 0)
                        break;
                }
                else break;
            }
        }

        private void CalculatePath()
        {
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                path.AddRange(MathVert.ConnectPoints(vertices[i + 1], vertices[i], false));
                //Drawer.DrawLine(edges[i], edges[i + 1], Vector3.up, Color.Lerp(Color.blue, Color.black, (float)i / edges.Count));
            }

            /*for (int i = 0; i < path.Count - 1; i++) 
            { 
                Drawer.DrawLine(path[i], path[i + 1], Color.Lerp(Color.white, Color.black, (float)i / path.Count)); 
            }*/
        }

        private void SetPathWidth()
        {
            List<Vector2Int> oldPath = new List<Vector2Int>(path);
            path.Clear();
            foreach (Vector2Int pathPoint in oldPath)
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        path.Add(pathPoint + new Vector2Int(x, y));
                    }
                }
            }
        }

        private Vertex FindLowestEdgeNear(Vertex vertex)
        {
            Dictionary<float, Vertex> distToVertex = new Dictionary<float, Vertex>();
            foreach (Vertex incVertex in vertex.incidentVertices)
            {
                float incDist = GetVertexCoastlineDist(incVertex);

                if (distToVertex.ContainsKey(incDist))
                {
                    if (Vector2.Distance(distToVertex[incDist], vertex) < Vector2.Distance(incVertex, vertex))
                    {
                        distToVertex[incDist] = incVertex;
                    }
                }
                else distToVertex[incDist] = incVertex;
            }

            return distToVertex[distToVertex.Keys.Min()];
        }

        private float GetVertexCoastlineDist(Vertex vertex)
        {
            float distance = 0;
            foreach (Region region in vertex.incidentRegions)
            {
                if (region.type.DistIndexFromCoastline != null)
                    distance += (int)region.type.DistIndexFromCoastline;

                if (region.type.isCoastline)
                    return 0;
            }
            return distance / vertex.incidentRegions.Count;
        }
    }
}

