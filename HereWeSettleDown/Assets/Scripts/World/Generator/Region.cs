using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Map;

namespace World.Generator
{
    public class Region
    {
        private Dictionary<Vector2Int, Vector2Int> ranges = new Dictionary<Vector2Int, Vector2Int>();

        public Vector2Int[] bounds;
        public Vector2Int[] edges;
        public Vector2 site;

        private readonly Vector2Int[] setColorPattern = new Vector2Int[] { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };

        public Region(Vector2Int[] bounds, Vector2Int[] edges, Vector2 site)
        {
            this.bounds = bounds;
            this.edges = edges;
            this.site = site;
            RecalculateRanges();
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

        public void Fill(Color color)
        {
            DoForEachPosition((Vector2Int point) => WorldMesh.colorMap[point.x, point.y].ALL = color);
            WorldMesh.ConfirmChanges();
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
}
 