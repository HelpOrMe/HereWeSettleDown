using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class VoronoiMesh
{
    public static void SetMesh(GameObject meshObject, Voronoi voronoi)
    {
        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();

        foreach (Circle circle in voronoi.Circles())
        {
            Vector2 point = circle.center;
            List<Vector2> region = voronoi.Region(point);
        }
    }
}

public class Region
{
    public Vector3 sitePos;
    public Vector3[] edgesPos;
    public Color color;

    public Vector2 site;
    public Vector2[] edges;

    public int[] verticesOffset;
    public Vector3[] verticesPositions;

    /*public Region()
    {

    }

    public Vector3[] GetVerticesPositions()
    {
        Vector3[] verticesPositions = new Vector3[verticesOffset.Length];
        foreach (int offset in verticesOffset)
        {

        }
    }*/

    public void SetVerticesOffset()
    {
        //    0          1          0         1         0
        // 0, 1, 2, | 0, 3, 2, | 0, 3, 4 | 0, 5, 4 | 0, 5, 6 ...

        verticesOffset = new int[edges.Length * 3];
        for (int i = 0; i < edges.Length * 3; i++)
        {
            // edges.Length = n
            // 0...n, 1...n, 1...n 
            int offset = i / (edges.Length + 1) + i % (edges.Length + 1);

            if (i % 3 == 0)
                verticesOffset[i] = 0;
            else if ((i / 3) % 2 == 0)
                verticesOffset[i] = offset + (2 * (offset / 3));
            else
                verticesOffset[i] = (3 - (offset % 3)) + (offset / 3);
        }
    }
}
