using UnityEngine;

namespace Debuger
{
    public static class Drawer
    {
        public static void DrawConnectedLines(Vector3[] points, Color color, bool conLastPoints = true, float duration = float.PositiveInfinity)
        {
            for (int i = 0; i < points.Length - 1; i++)
                Debug.DrawLine(points[i], points[i + 1], color, duration);

            if (conLastPoints)
                Debug.DrawLine(points[0], points[points.Length - 1], color, duration);
        }

        public static void DrawHLine(Vector2 point, Color color, float duration = float.PositiveInfinity)
        {
            DrawHLine(ToVector3(point), color, duration);
        }

        public static void DrawHLine(Vector3 point, Color color, float duration = float.PositiveInfinity)
        {
            Debug.DrawLine(point + Vector3.down * 3, point + Vector3.up * 3, color, duration);
        }

        public static void DrawLine(Vector2 point1, Vector2 point2, Color color, float duration = float.PositiveInfinity)
        {
            Debug.DrawLine(ToVector3(point1), ToVector3(point2), color, duration);
        }

        private static Vector3 ToVector3(Vector2 p)
        {
            return new Vector3(p.x, 0.1f, p.y);
        }
    }
}

