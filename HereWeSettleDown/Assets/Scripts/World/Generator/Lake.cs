using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Helper.Threading;
using Helper.Debugger;

namespace World.Generator
{
    public class Lake
    {
        public readonly Edge startEdge;
        public readonly List<Edge> edges = new List<Edge>();

        public Lake(Edge startEdge)
        {
            this.startEdge = startEdge;
            CalculateEdges();
        }

        public void CalculateEdges()
        {
            Drawer.DrawHLine(startEdge, Color.blue);

            List<Edge> lakePath = new List<Edge>();
            Edge lastEdge = startEdge;

            while (true)
            {
                Edge newEdge = FindLowestEdgeNear(lastEdge);
                if (newEdge != null)
                {
                    Drawer.DrawLine(lastEdge, newEdge, Color.blue);
                    lakePath.Add(newEdge);
                    lastEdge = newEdge;
                    if (GetEdgeCoastlineDist(newEdge) <= 0)
                        break;
                }
                else break;
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

