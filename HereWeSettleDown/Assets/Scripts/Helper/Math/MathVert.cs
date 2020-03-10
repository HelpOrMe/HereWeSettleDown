using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Helper.Math
{
    public static class MathVert
    {
        public static float GetHeihtBetween3Points(Vector2 targetPoint, Vector3[] points)
        {
            if (points.Length < 3)
                return 0;
            return GetHeihtBetween3Points(targetPoint, points[0], points[1], points[2]);
        }

        public static float GetHeihtBetween3Points(Vector2 targetPoint, Vector3 point1, Vector3 point2, Vector3 point3)
        {
            // Barycentric coordinate system / Conversion between barycentric and Cartesian coordinates
            // https://en.wikipedia.org/wiki/Barycentric_coordinate_system#Conversion_between_barycentric_and_Cartesian_coordinates

            float x, y;
            float x1, z1, y1;
            float x2, z2, y2;
            float x3, z3, y3;

            x = targetPoint.x; y = targetPoint.y;
            x1 = point1.x; z1 = point1.y; y1 = point1.z;
            x2 = point2.x; z2 = point2.y; y2 = point2.z;
            x3 = point3.x; z3 = point3.y; y3 = point3.z;
            
            // Calculate delta's

            float d10 = (y2 - y3) * (x - x3) + (x3 - x2) * (y - y3);
            float d11 = (y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3);
            float d1 = d10 / d11;

            float d20 = (y3 - y1) * (x - x3) + (x1 - x3) * (y - y3);
            float d21 = (y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3);
            float d2 = d20 / d21;

            float d3 = 1 - d1 - d2;

            float z = d1 * z1 + d2 * z2 + d3 * z3;
            
            return z;
        }

        public static Vector2Int[] GetPositionsBetween(Vector2[] points)
        {
            return GetPositionsBetween(GetRangesBetween(points));
        }

        public static Vector2Int[] GetPositionsBetween(Dictionary<Vector2Int, Vector2Int> ranges)
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            foreach (Vector2Int leftPoint in ranges.Keys)
            {
                for (int x = leftPoint.x; x <= ranges[leftPoint].x; x++)
                {
                    positions.Add(new Vector2Int(x, leftPoint.y));
                }
            }

            return positions.ToArray();
        }

        public static Dictionary<Vector2Int, Vector2Int> GetRangesBetween(Vector2[] points)
        {
            return GetRangesBetween(GetBoundsBetween(points));
        }

        public static Dictionary<Vector2Int, Vector2Int> GetRangesBetween(Vector2Int[] bounds)
        {
            Dictionary<Vector2Int, Vector2Int> ranges = new Dictionary<Vector2Int, Vector2Int>();

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

            return ranges;
        }

        public static Vector2Int[] GetBoundsBetween(Vector2[] points)
        {
            List<Vector2Int> bounds = new List<Vector2Int>();
            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;
                Vector2Int[] connectedPoints = ConnectPoints(points[i], points[j], true);
                bounds.AddRange(connectedPoints);
            }
            return bounds.ToArray();
        }

        public static Vector2Int[] ConnectPoints(Vector2 point1, Vector2 point2, bool connectLastPoint)
        {
            List<Vector2Int> points = new List<Vector2Int>();

            point1 = RoundPosition(point1);
            point2 = RoundPosition(point2);

            Vector2 offset = point1 - point2;
            float dist = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y)) + 1;

            for (float i = 0; i <= dist; i += 1 / dist)
            {
                Vector2 point = RoundPosition(Vector2.Lerp(point1, point2, i));
                Vector2Int intPoint = ToVector2Int(point);

                if (!points.Contains(intPoint))
                    points.Add(intPoint);
            }

            if (connectLastPoint)
                points.Add(ToVector2Int(point1));

            return points.ToArray();
        }

        public static Vector3[] ConnectPoints(Vector3 point1, Vector3 point2, bool connectLastPoint)
        {
            List<Vector3> points = new List<Vector3>();

            point1 = RoundPosition(point1);
            point2 = RoundPosition(point2);

            Vector3 offset = point1 - point2;
            float dist = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.z)) + 1;

            for (float i = 0; i <= dist; i += 1 / dist)
            {
                Vector3 point = RoundPosition(Vector3.Lerp(point1, point2, i));

                if (!points.Contains(point))
                    points.Add(point);
            }

            if (connectLastPoint)
                points.Add(RoundPosition(point1));

            return points.ToArray();
        }

        public static Vector3 RoundPosition(Vector3 p)
        {
            return new Vector3(Mathf.Round(p.x), p.y, Mathf.Round(p.z));
        }

        public static Vector2 RoundPosition(Vector2 p)
        {
            return new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
        }

        public static Vector2Int RoundPosition(Vector2Int p)
        {
            return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
        }

        public static Vector2Int[] ToVector2Int(IEnumerable<Vector2> ps)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            foreach (Vector2 p in ps)
                points.Add(ToVector2Int(p));
            return points.ToArray();
        }

        public static Vector2Int[] ToVector2Int(IEnumerable<Vector3> ps)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            foreach (Vector2 p in ps)
                points.Add(ToVector2Int(p));
            return points.ToArray();
        }

        public static Vector2Int ToVector2Int(Vector3 p)
        {
            return ToVector2Int(ToVector2(p));
        }

        public static Vector2Int ToVector2Int(Vector2 p)
        {
            return new Vector2Int((int)p.x, (int)p.y);
        }

        public static Vector2 ToVector2(Vector3 p)
        {
            return new Vector2(p.x, p.z);
        }

        public static Vector3 ToVector3(Vector2 p)
        {
            return new Vector3(p.x, 0, p.y);
        }
    }
}
