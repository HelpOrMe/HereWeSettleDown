using UnityEngine;

namespace World.Map
{
    public class ChunkMeshData
    {
        public Quad[,] quadMap;

        public int width;
        public int height;

        public int chunkX;
        public int chunkY;

        public int[] triangles;
        public Vector3[] vertices;
        public Vector2[] uv;
        public Color[] colors;

        public static readonly int[] trianglesPattern = new int[] { 0, 1, 2, 3, 4, 5 };
        public static readonly int[] verticesIndexPattern = new int[] { 1, 3, 0, 3, 1, 2};
        public static readonly Vector2Int[] verticesPositionPattern = new Vector2Int[] { Vector2Int.zero, Vector2Int.up, Vector2Int.one, Vector2Int.right };


        public ChunkMeshData(int width, int height, int x, int y)
        {
            this.width = width;
            this.height = height;

            chunkX = x;
            chunkY = y;

            quadMap = new Quad[width, height];

            int size = width * height * 6;
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
                    int sVertInd = (y * width + x) * 6;
                    for (int i = 0; i < 6; i++)
                    {
                        triangles[sVertInd + i] = sVertInd + trianglesPattern[i];
                        Vector2Int targetVertPosition = verticesPositionPattern[verticesIndexPattern[i]];
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
            int sVertInd = (y * width + x) * 6;
            for (int i = 0; i < 6; i++)
            {
                vertices[sVertInd + i] = quadMap[x, y].GetVert(verticesIndexPattern[i]);
                colors[sVertInd + i] = quadMap[x, y].GetColor(i > 2 ? 1 : 0);
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
        readonly int x;
        readonly int y;

        public Quad(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector3 GetVert(int index)
        {
            Vector2Int targetVert = ChunkMeshData.verticesPositionPattern[index];
            return WorldMeshMap.verticesMap[x + targetVert.x, y + targetVert.y];
        }

        public Color GetColor(int index)
        {
            return WorldMeshMap.colorMap[x, y, index];
        }
    }

    public struct ColorPack
    {
        public Color fColor;
        public Color sColor;

        public Color this[int index]
        {
            get
            {
                return index == 0 ? fColor : sColor;
            }
            set
            {
                if (index == 0)
                    fColor = value;
                else
                    sColor = value;
            }
        }

        public ColorPack(Color f, Color s)
        {
            fColor = f;
            sColor = s;
        }
    }
}
