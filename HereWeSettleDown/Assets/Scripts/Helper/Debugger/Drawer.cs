using Helper.Math;
using Helper.Threading;
using UnityEngine;

namespace Helper.Debugging
{
    /// <summary>
    /// Draw helper.
    /// </summary>
    public static class Drawer
    {
        /// <summary>
        /// Draw lines between points.
        /// </summary>
        public static void DrawConnectedLines(Vector3[] points, Color color, bool conLastPoints = false, float duration = float.PositiveInfinity)
        {
            for (int i = 0; i < points.Length - 1; i++)
                DefDrawLine(points[i], points[i + 1], color, duration);

            if (conLastPoints)
                DefDrawLine(points[0], points[points.Length - 1], color, duration);
        }

        /// <summary>
        /// Draw lines between points.
        /// </summary>
        public static void DrawConnectedLines(Vector2[] points, Color color, bool conLastPoints = false, float duration = float.PositiveInfinity)
        {
            for (int i = 0; i < points.Length - 1; i++)
                DrawLine(points[i], points[i + 1], color, duration);

            if (conLastPoints)
                DrawLine(points[0], points[points.Length - 1], color, duration);
        }

        /// <summary>
        /// Draw lines between points.
        /// </summary>
        public static void DrawConnectedLines(Vector2Int[] points, Color color, bool conLastPoints = false, float duration = float.PositiveInfinity)
        {
            for (int i = 0; i < points.Length - 1; i++)
                DrawLine(points[i], points[i + 1], color, duration);

            if (conLastPoints)
                DrawLine(points[0], points[points.Length - 1], color, duration);
        }

        /// <summary>
        /// Draw vertical line.
        /// </summary>
        public static void DrawVLine(Vector2 point, Color color, float length, float duration = float.PositiveInfinity)
        {
            DrawVLine(MathVert.ToVector3(point), color, length, duration);
        }

        /// <summary>
        /// Draw vertical line.
        /// </summary>
        public static void DrawVLine(Vector3 point, Color color, float length, float duration = float.PositiveInfinity)
        {
            DefDrawLine(point + Vector3.down * length, point + Vector3.up * length, color, duration);
        }

        /// <summary>
        /// Draw line between two points.
        /// </summary>
        public static void DrawLine(Vector2 point1, Vector2 point2, Color color, float duration = float.PositiveInfinity)
        {
            DefDrawLine(MathVert.ToVector3(point1), MathVert.ToVector3(point2), color, duration);
        }

        /// <summary>
        /// Draw line between two points.
        /// </summary>
        public static void DrawLine(Vector3 point1, Vector3 point2, Color color, float duration = float.PositiveInfinity)
        {
            DefDrawLine(point1, point2, color, duration);
        }

        /// <summary>
        /// Draw line between two points.
        /// </summary>
        public static void DrawLine(Vector2 point1, Vector2 point2, Vector3 offset, Color color, float duration = float.PositiveInfinity)
        {
            DefDrawLine(MathVert.ToVector3(point1) + offset, MathVert.ToVector3(point2) + offset, color, duration);
        }

        /// <summary>
        /// Default draw line with Debug.DrawLine. Works in subthreads
        /// </summary>
        public static void DefDrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            if (!MainThreadInvoker.CheckForMainThread())
                MainThreadInvoker.InvokeAction(() => Debug.DrawLine(start, end, color, duration));
            else
                Debug.DrawLine(start, end, color, duration);
        }
    }
}
