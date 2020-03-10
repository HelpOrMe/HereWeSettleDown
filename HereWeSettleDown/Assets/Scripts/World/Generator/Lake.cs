using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Helper.Math;
//using Helper.Debugger;

namespace World.Generator
{
    public class Lake
    {
        public readonly Edge startEdge;
        public readonly List<Edge> edges = new List<Edge>();
        public readonly List<Vector2Int> path = new List<Vector2Int>();

        public Lake(Edge startEdge) => this.startEdge = startEdge;

        public void Set()
        {
            CalculateEdges();
            CalculatePath();
            SetPathWidth();
        }

        private void CalculateEdges()
        {
            //Drawer.DrawHLine(startEdge, Color.blue);
            edges.Clear();
            Edge lastEdge = startEdge;
            for (int i = 0; i < 100; i++) // while
            {
                Edge newEdge = FindLowestEdgeNear(lastEdge);
                if (newEdge != null)
                {
                    //Drawer.DrawHLine(newEdge, Color.blue);
                    //Drawer.DrawLine(lastEdge, newEdge, Color.blue);
                    edges.Add(newEdge);
                    lastEdge = newEdge;
                    if (GetEdgeCoastlineDist(newEdge) <= 0)
                        break;
                }
                else break;
            }
        }

        private void CalculatePath()
        {
            for (int i = 0; i < edges.Count - 1; i++)
            {
                path.AddRange(MathVert.ConnectPoints(edges[i + 1], edges[i], true));
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

        private Edge FindLowestEdgeNear(Edge edge)
        {
            Dictionary<float, Edge> distToEdge = new Dictionary<float, Edge>();
            foreach (Edge incEdge in edge.incidentEdges)
            {
                float incDist = GetEdgeCoastlineDist(incEdge);

                if (distToEdge.ContainsKey(incDist))
                {
                    if (Vector2.Distance(distToEdge[incDist], edge) < Vector2.Distance(incEdge, edge))
                    {
                        distToEdge[incDist] = incEdge;
                    }
                }
                else distToEdge[incDist] = incEdge;
            }

            return distToEdge[distToEdge.Keys.Min()];
        }

        private float GetEdgeCoastlineDist(Edge edge)
        {
            float distance = 0;
            foreach (Region region in edge.incidentRegions)
            {
                if (region.type.DistIndexFromCoastline != null)
                    distance += (int)region.type.DistIndexFromCoastline;

                if (region.type.isCoastline)
                    return 0;
            }
            return distance / edge.incidentRegions.Count;
        }
    }
}

