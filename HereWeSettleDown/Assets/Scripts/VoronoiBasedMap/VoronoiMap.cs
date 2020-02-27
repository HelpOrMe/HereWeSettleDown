using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class VoronoiMap : MonoBehaviour
{
    public int seed = 0;
    public int cellsCount = 124;

    public int mapWidth = 124;
    public int mapHeight = 124;

    public int chunkWidth = 124 / 2;
    public int chunkHeight = 124 / 2;

    public GameObject meshObject;

    public static Voronoi voronoi;
    private System.Random prng;

    private void Awake()
    {
        prng = new System.Random(seed);
    }

    private void Start()
    {
        SetVoronoi();
        ShowVoronoi();
        VoronoiMesh.CreateMesh(mapWidth, mapHeight, chunkWidth, chunkHeight);
        VoronoiMesh.SetMesh(meshObject);
    }

    private void SetVoronoi()
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < cellsCount; i ++)
        {
            points.Add(new Vector2(prng.Next(0, mapWidth), prng.Next(0, mapHeight)));
        }
        voronoi = new Voronoi(points, new Rectf(0, 0, mapWidth, mapHeight), 2, prng);
    }

    private void ShowVoronoi()
    {
        foreach (LineSegment line in voronoi.VoronoiDiagram())
        {
            Debug.Log(ScalePosition(line.p0) + " " + ScalePosition(line.p1));
            DrawLine(ScalePosition(line.p0), ScalePosition(line.p1), Color.red, 999);
        }

        foreach (Vector2 point in voronoi.SiteCoords())
        {
            Color color = Random.ColorHSV();
            
            DrawHLine(ScalePosition(point), color);
            /*foreach (Vector2 regionPoint in voronoi.Region(point))
            {
                DrawLine(ScalePosition(regionPoint), ScalePosition(point), color, 999);
                DrawHLine(ScalePosition(regionPoint), color);
            }*/
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

    private Vector2 ScalePosition(Vector2 p)
    {
        return new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
    }
}
