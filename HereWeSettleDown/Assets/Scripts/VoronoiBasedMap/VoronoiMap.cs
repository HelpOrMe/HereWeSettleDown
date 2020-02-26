using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class VoronoiMap : MonoBehaviour
{
    public int seed = 0;
    public int width = 124;
    public int height = 124;
    public int cellsCount = 124;

    public GameObject meshObject;

    public static Voronoi voronoi;
    private System.Random prng;

    private void Awake()
    {
        Debug.Log(5 / 3);
        prng = new System.Random(seed);
    }

    private void Start()
    {
        SetVoronoi();
        ShowVoronoi();
    }

    private void SetVoronoi()
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < cellsCount; i ++)
        {
            points.Add(new Vector2(prng.Next(0, width), prng.Next(0, width)));
        }
        voronoi = new Voronoi(points, new Rectf(0, 0, width, height), 2, prng);
    }

    private void ShowVoronoi()
    {
        foreach (LineSegment line in voronoi.VoronoiDiagram())
        {
            DrawLine(line.p0, line.p1, Color.white, 999);
        }

        foreach (Vector2 point in voronoi.SiteCoords())
        {
            Color color = Random.ColorHSV();
            
            DrawHLine(point, color);
            foreach (Vector2 regionPoint in voronoi.Region(point))
            {
                DrawLine(regionPoint, point, color, 999);
                DrawHLine(regionPoint, color);
            }
        }
    }
    
    private void DrawHLine(Vector2 point, Color color, int duration = 999)
    {
        Debug.DrawLine(ToVector3(point) + Vector3.down * 4, ToVector3(point) + Vector3.up * 4, color, duration);
    }

    private void DrawLine(Vector2 point1, Vector2 point2, Color color, int duration = 999)
    {
        Debug.DrawLine(ToVector3(point1), ToVector3(point2), color, duration);
    }

    private Vector3 ToVector3(Vector2 p)
    {
        return new Vector3(p.x, 0, p.y);
    }
}
