using UnityEngine;

namespace Generator
{
    public static class MeshEditor
    {
        public static MeshData GenerateNewMesh(int seed, float[,] heightMap, Vector2Int xRandomRange, Vector2Int yRandomRange, 
            Vector2 triangleScale, float heightMultiplier, AnimationCurve heightCurve, ColorPack[,] colorMap)
        {
            int width = heightMap.GetLength(0) - 1;
            int height = heightMap.GetLength(1) - 1;

            // Generate random offset map
            Vector3[,] offsetMap = GenerateRandomOffset(seed, width + 1, height + 1, xRandomRange, yRandomRange);

            // Generate mesh map
            MeshData meshData = new MeshData(width + 1, height + 1);

            int lastVert = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Create two triangles
                    for (int i = 0; i < 2; i++)
                    {
                        int[] order;
                        if (i == 0)
                            order = new int[3] { 0, 1, 2 };
                        else
                            order = new int[3] { 1, 0, 2 };

                        int[] vertIndexes = new int[3] 
                        { 
                            lastVert + order[0], 
                            lastVert + order[1], 
                            lastVert + order[2]
                        };
                        Vector3[] vertPositions = new Vector3[3] 
                        {
                            new Vector3(x, heightCurve.Evaluate(heightMap[x, y + 1]), y + 1) + offsetMap[x, y + 1],
                            new Vector3(x + 1, heightCurve.Evaluate(heightMap[x + 1, y]), y) + offsetMap[x + 1, y],
                            new Vector3(x + i, heightCurve.Evaluate(heightMap[x + i, y + i]), y + i) + offsetMap[x + i, y + i]
                        };
                        
                        Vector2[] uv = new Vector2[3]
                        {
                            new Vector2(x / (float)width, (y + 1) / (float)height),
                            new Vector2((x + 1) / (float)width, y / (float)height),
                            new Vector2((x + i) / (float)width, (y + i) / (float)height)
                        };

                        // Scale vertPositions
                        for (int j = 0; j < 3; j++)
                            vertPositions[j] = Vector3.Scale(vertPositions[j], new Vector3(triangleScale.x, heightMultiplier, triangleScale.y));

                        meshData.AddTriangle(vertIndexes, vertPositions, uv, colorMap[x, y][i]);
                        lastVert += 3;
                    }
                }
            }
            return meshData;
        }

        static Vector3[,] GenerateRandomOffset(int seed, int width, int height, Vector2Int xRandRange, Vector2Int yRandRange)
        {
            System.Random prng = new System.Random(seed);

            Vector3[,] offset = new Vector3[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    offset[x, y] = new Vector3(prng.Next(xRandRange.x, xRandRange.y) / 100f, 0, prng.Next(yRandRange.x, yRandRange.y) / 100f);
                }
            }

            return offset;
        }
    }

    public class MeshData
    {
        //public Quad[,] map;

        public int[] triangles;
        public Vector3[] vertices;
        public Vector2[] uv;
        public Color[] colors;

        int lastAdd = 0;

        public MeshData(int width, int height)
        {
            int size = (width - 1) * (height - 1) * 6;

            vertices = new Vector3[size];
            triangles = new int[size];
            uv = new Vector2[size];
            colors = new Color[size];

            //map = new Quad[width - 1, height - 1];
        }

        public void AddTriangle(int[] vertIndexes, Vector3[] vertPositions, Vector2[] _uv, Color[] _colors)
        {
            for (int i = 0; i < 3; i ++)
            {
                triangles[lastAdd + i] = vertIndexes[i];
                vertices[lastAdd + i] = vertPositions[i];
                uv[lastAdd + i] = _uv[i];
                colors[lastAdd + i] = _colors[i];
            }
            lastAdd += 3;
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

    public struct Quad
    {
        public Triangle[] triangles;

        public Quad(Triangle first, Triangle second)
        {
            triangles = new Triangle[2] { first, second };
        }
    }

    public struct Triangle
    {
        public Vertex[] vertices;

        public Triangle(Vertex[] vertices)
        {
            this.vertices = vertices;
        }

        public Triangle(int[] indexes, Vector3[] positions)
        {
            vertices = new Vertex[3];
            for (int i = 0; i < 3; i++)
                vertices[i] = new Vertex(positions[i], this, Color.black, indexes[i]);
        }
    }

    public struct Vertex
    {
        public Vector3 position;
        public Triangle parent;
        public Color color;
        public int index;

        public Vertex(Vector3 position, Triangle parent, Color color, int index)
        {
            this.position = position;
            this.parent = parent;
            this.color = color;
            this.index = index;
        }
    }

    public class ColorPack
    {
        public Color[] firstColors;
        public Color[] secondColors;

        public ColorPack()
        {
            firstColors = new Color[3] { Color.black, Color.black, Color.black };
            secondColors = new Color[3] { Color.black, Color.black, Color.black };
        }

        public ColorPack(Color[] fColors, Color[] sColors)
        {
            firstColors = fColors;
            secondColors = sColors;
        }

        public ColorPack(Color fColor, Color sColor)
        {
            firstColors = new Color[3] { fColor, fColor, fColor };
            secondColors = new Color[3] { sColor, sColor, sColor };
        }

        public Color[] this[int index]
        {
            get
            {
                if (index == 0)
                    return firstColors;
                else
                    return secondColors;
            }
            set
            {
                if (index == 0)
                    firstColors = value;
                else
                    secondColors = value;
            }
        }
    }
}

