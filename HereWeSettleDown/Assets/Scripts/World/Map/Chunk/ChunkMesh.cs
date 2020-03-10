using UnityEngine;

namespace World.Map
{
    public class ChunkMesh
    {
        private Chunk connectedChunk;
        
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

        public void ConnectChunk(Chunk chunk)
        {
            connectedChunk = chunk;
        }

        public void UpdateConnectedChunk()
        {
            if (connectedChunk != null)
                connectedChunk.DrawMesh();
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
            return WorldMesh.verticesMap[x + targetVert.x, y + targetVert.y];
        }

        public Color GetColor(int index)
        {
            return WorldMesh.colorMap[x, y][index];
        }

        public void RecalculateMiddleVert()
        {
            Vector3 sumOfQuadPoints = Vector3.zero;
            for (int i = 1; i < 5; i++)
            {
                Vector2Int mapPosition = ChunkMesh.verticesMapPositionPattern[i];
                Vector3 vert = WorldMesh.verticesMap[x + mapPosition.x, y + mapPosition.y];
                sumOfQuadPoints += vert;
            }
            middleVert = Vector3.Scale(sumOfQuadPoints, new Vector3(0.25f, 0.25f, 0.25f));
        }
    }

    public class ColorQuad
    {
        private int x, y;

        public Color[] colors = new Color[4];
        public Color this[int index]
        {
            get { return colors[index]; }
            set { colors[index] = value; SetEditedPosition(); }
        }

        public Color DOWN
        {
            get { return colors[3]; }
            set { colors[3] = value; SetEditedPosition(); }
        }
        public Color LEFT
        {
            get { return colors[0]; }
            set { colors[0] = value; SetEditedPosition(); }
        }
        public Color UP
        {
            get { return colors[1]; }
            set { colors[1] = value; SetEditedPosition(); }
        }
        public Color RIGHT
        {
            get { return colors[2]; }
            set { colors[2] = value; SetEditedPosition(); }
        }

        public Color ALL
        {
            set
            {
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = value;
                SetEditedPosition();
            }
        }

        public ColorQuad(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void SetEditedPosition()
        {
            if (x >= 0 && y >= 0)
                WorldMesh.SetEditedPosition(x, y);
        }

        public void Smooth()
        {
            Smooth01();
            Smooth23();
            Smooth12();
            Smooth30();
        }

        public void Smooth01()
        {
            if (DOWN == LEFT)
            {
                ColorQuad leftQuad = WorldMesh.colorMap[x - 1, y];
                ColorQuad downQuad = WorldMesh.colorMap[x, y - 1];

                if (leftQuad.RIGHT == downQuad.UP)
                {
                    DOWN = leftQuad.RIGHT;
                    LEFT = leftQuad.RIGHT;
                }
            }
        }

        public void Smooth23()
        {
            if (UP == RIGHT)
            {
                ColorQuad upQuad = WorldMesh.colorMap[x, y + 1];
                ColorQuad rightQuad = WorldMesh.colorMap[x + 1, y];

                if (upQuad.DOWN == rightQuad.LEFT)
                {
                    UP = upQuad.DOWN;
                    RIGHT = upQuad.DOWN;
                }
            }
        }

        public void Smooth12()
        {
            if (LEFT == UP)
            {
                ColorQuad upQuad = WorldMesh.colorMap[x, y + 1];
                ColorQuad leftQuad = WorldMesh.colorMap[x - 1, y];

                if (upQuad.DOWN == leftQuad.RIGHT)
                {
                    UP = upQuad.DOWN;
                    LEFT = upQuad.DOWN;
                }
            }
        }

        public void Smooth30()
        {
            if (RIGHT == DOWN)
            {
                ColorQuad rightQuad = WorldMesh.colorMap[x + 1, y];
                ColorQuad downQuad = WorldMesh.colorMap[x, y - 1];

                if (rightQuad.LEFT == downQuad.UP)
                {
                    RIGHT = rightQuad.LEFT;
                    DOWN = rightQuad.LEFT;
                }
            }
        }
    }
}
