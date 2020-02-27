using UnityEngine;

public static class VoronoiMesh
{
    public static ChunkMesh[,] chunkMeshMap;
    public static Vector3[,] verticesMap;
    public static ColorQuad[,] colorMap;

    public static void CreateMesh(int width, int height, int chunkWidth, int chunkHeight)
    {
        SetEmptyVerticesMap(width, height);
        SetEmptyColorMap(width, height);

        int chunkXCount = width / chunkWidth;
        int chunkYCount = height / chunkHeight;

        chunkMeshMap = new ChunkMesh[chunkXCount, chunkYCount];
        for (int x = 0; x < chunkXCount; x++)
        {
            for (int y = 0; y < chunkYCount; y++)
            {
                chunkMeshMap[x, y] = new ChunkMesh(chunkWidth, chunkHeight, x, y);
            }
        }
    }

    public static void SetMesh(GameObject chunkObject)
    {
        if (!chunkObject.GetComponent<MeshFilter>())
            return;

        for (int x = 0; x < chunkMeshMap.GetLength(0); x++)
        {
            for (int y = 0; y < chunkMeshMap.GetLength(1); y++)
            {
                Mesh mesh = chunkMeshMap[x, y].CreateMesh();
                GameObject newChunkObject = Object.Instantiate(chunkObject);
                newChunkObject.GetComponent<MeshFilter>().sharedMesh = mesh;
            }
        }
    }

    public static void SetEmptyVerticesMap(int width, int height)
    {
        verticesMap = new Vector3[width + 1, height + 1];
        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                verticesMap[x, y] = new Vector3(x, 0, y);
            }
        }
    }

    public static void SetEmptyColorMap(int width, int height)
    {
        colorMap = new ColorQuad[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                colorMap[x, y] = new ColorQuad();
            }
        }
    }
}

public class ChunkMesh
{
    public int chunkX, chunkY;
    public int width, height;
    public Quad[,] quadMap;

    public int[] triangles;
    public Vector3[] vertices;
    public Vector2[] uv;
    public Color[] colors;

    public static readonly int[] verticesIndexPattern = new int[] { 2, 0, 1, 0, 2, 3, 4, 0, 3, 0, 4, 1 };
    public static readonly Vector2[] verticesPositionPattern = new Vector2[] { new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
    public static readonly Vector2Int[] verticesMapPositionPattern = new Vector2Int[] { Vector2Int.one, Vector2Int.zero, Vector2Int.up, Vector2Int.one, Vector2Int.right };

    public ChunkMesh(int width, int height, int x, int y)
    {
        quadMap = new Quad[width, height];
        this.width = width; 
        this.height = height;
        chunkX = x; 
        chunkY = y;

        int size = width * height * 12;
        triangles = new int[size];
        vertices = new Vector3[size];
        uv = new Vector2[size];
        colors = new Color[size];

        SetDefaultTriangles();
        SetQuadMap();
    }

    public void SetDefaultTriangles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int sVertInd = (y * width + x) * 12;
                for (int i = 0; i < 12; i++)
                {
                    triangles[sVertInd + i] = sVertInd + i;
                    Vector2 targetVertPosition = verticesPositionPattern[verticesIndexPattern[i]];
                    uv[sVertInd + i] = new Vector2((x + targetVertPosition.x) / (width + 1), (y + targetVertPosition.y) / (height + 1));
                }
            }
        }
    }

    public void SetQuadMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int aX = chunkX * width + x;
                int aY = chunkY * height + y;
                quadMap[x, y] = new Quad(aX, aY);
                UpdateQuadValues(x, y);
            }
        }
    }

    public void UpdateQuadValues(int x, int y)
    {
        int sVertInd = (y * width + x) * 12;
        
        quadMap[x, y].RecalculateMiddleVert();
        for (int i = 0; i < 12; i++)
        {
            vertices[sVertInd + i] = quadMap[x, y].GetVert(verticesIndexPattern[i]);
            colors[sVertInd + i] = quadMap[x, y].GetColor(i / 3);
        }
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uv,
            colors = colors
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}

public class Quad
{
    readonly int x, y;
    private Vector3 middleVert;

    public Quad(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector3 GetVert(int index)
    {
        if (index == 0)
            return middleVert;

        Vector2Int targetVert = ChunkMesh.verticesMapPositionPattern[index];
        return VoronoiMesh.verticesMap[x + targetVert.x, y + targetVert.y];
    }

    public Color GetColor(int index)
    {
        return VoronoiMesh.colorMap[x, y][index];
    }

    public void RecalculateMiddleVert()
    {
        Vector3 sumOfQuadPoints = Vector3.zero;
        for (int i = 1; i < 5; i ++)
        {
            Vector2Int mapPosition = ChunkMesh.verticesMapPositionPattern[i];
            sumOfQuadPoints += VoronoiMesh.verticesMap[x + mapPosition.x, y + mapPosition.y];
        }
        middleVert = Vector3.Scale(sumOfQuadPoints, new Vector3(0.25f, 0.25f, 0.25f));
    }
}

public class ColorQuad
{
    public Color[] colors = new Color[4];
    public Color this[int index]
    {
        get { return colors[index]; }
        set { colors[index] = value; }
    }

    public Color DOWN
    {
        get { return colors[0]; }
        set { colors[0] = value; }
    }
    public Color LEFT
    {
        get { return colors[1]; }
        set { colors[1] = value; }
    }
    public Color UP
    {
        get { return colors[2]; }
        set { colors[2] = value; }
    }
    public Color RIGHT
    {
        get { return colors[3]; }
        set { colors[3] = value; }
    }
}

/*public class Region
{
    public Vector3 sitePos;
    public Vector3[] edgesPos;
    public Color color;

    public Vector2 site;
    public Vector2[] edges;

    public int[] verticesOffset;
    public Vector3[] verticesPositions;
}*/
