using System.Threading;
using UnityEngine;
using Helper.Threading;

namespace Helper.Debugger
{
    public static class Drawer
    {
        public static void DrawConnectedLines(Vector3[] points, Color color, bool conLastPoints = false, float duration = float.PositiveInfinity)
        {
            for (int i = 0; i < points.Length - 1; i++)
                DefDrawLine(points[i], points[i + 1], color, duration);

            if (conLastPoints)
                DefDrawLine(points[0], points[points.Length - 1], color, duration);
        }

        public static void DrawConnectedLines(Vector2Int[] points, Color color, bool conLastPoints = false, float duration = float.PositiveInfinity)
        {
            for (int i = 0; i < points.Length - 1; i++)
                DrawLine(points[i], points[i + 1], color, duration);

            if (conLastPoints)
                DrawLine(points[0], points[points.Length - 1], color, duration);
        }

        public static void DrawHLine(Vector2 point, Color color, float duration = float.PositiveInfinity)
        {
            DrawHLine(ToVector3(point), color, duration);
        }

        public static void DrawHLine(Vector3 point, Color color, float duration = float.PositiveInfinity)
        {
            DefDrawLine(point + Vector3.down * 3, point + Vector3.up * 3, color, duration);
        }

        public static void DrawLine(Vector2 point1, Vector2 point2, Color color, float duration = float.PositiveInfinity)
        {
            DefDrawLine(ToVector3(point1), ToVector3(point2), color, duration);
        }

        public static void DrawLine(Vector2 point1, Vector2 point2, Vector3 offset, Color color, float duration = float.PositiveInfinity)
        {
            DefDrawLine(ToVector3(point1) + offset, ToVector3(point2) + offset, color, duration);
        }

        private static Vector3 ToVector3(Vector2 p)
        {
            return new Vector3(p.x, 0.1f, p.y);
        }

        private static void DefDrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            if (MainThreadInvoker.CheckForMainThread())
                Debug.DrawLine(start, end, color, duration);
            else
                MainThreadInvoker.InvokeAction(() => Debug.DrawLine(start, end, color, duration));
        }
    }
}
