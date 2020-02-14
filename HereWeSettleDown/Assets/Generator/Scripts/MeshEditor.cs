using UnityEngine;

namespace Generator
{
    public static class MeshEditor
    {
        public static MeshData[,] GenerateChunkMeshes(int chunkWidth, int chunkHeight, int chunkXCount, int chunkYCount, float[,] heightMap, Vector2Int xRandomRange, Vector2Int yRandomRange,
            Vector2 triangleScale, float heightMultiplier, AnimationCurve heightCurve, ColorPack[,] colorMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            chunkWidth--;
            chunkHeight--;

            // Generate random offset map
            Vector3[,] offsetMap = GenerateRandomOffset(width, height, xRandomRange, yRandomRange);

            // Generate all meshes
            MeshData[,] meshesMap = new MeshData[chunkXCount, chunkYCount];
            
            for (int cX = 0; cX < chunkXCount; cX++)
            {
                for (int cY = 0; cY < chunkYCount; cY++)
                {
                    // Generate chunk mesh
                    MeshData meshData = new MeshData(chunkWidth + 1, chunkHeight + 1);
                    int lastVert = 0;
                    for (int x = cX * chunkWidth ; x < cX * chunkWidth  + chunkWidth ; x++)
                    {
                        for (int y = cY * chunkHeight ; y < cY * chunkHeight  + chunkHeight ; y++)
                        {
                            int px = x % chunkWidth ;
                            int py = y % chunkHeight ;

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
                                    new Vector3(px, heightCurve.Evaluate(heightMap[x, y + 1]), py + 1) + offsetMap[x, y + 1],
                                    new Vector3(px + 1, heightCurve.Evaluate(heightMap[x + 1, y]), py) + offsetMap[x + 1, y],
                                    new Vector3(px + i, heightCurve.Evaluate(heightMap[x + i, y + i]), py + i) + offsetMap[x + i, y + i]
                                };

                                Vector2[] uv = new Vector2[3]
                                {
                                    new Vector2(x / (float)(width), (y + 1) / (float)(height)),
                                    new Vector2((x + 1) / (float)(width), y / (float)(height)),
                                    new Vector2((x + i) / (float)(width), (y + i) / (float)(height))
                                };

                                // Scale vertPositions
                                for (int j = 0; j < 3; j++)
                                    vertPositions[j] = Vector3.Scale(vertPositions[j], new Vector3(triangleScale.x, heightMultiplier, triangleScale.y));

                                meshData.AddTriangle(vertIndexes, vertPositions, uv, colorMap[x, y][i]);
                                lastVert += 3;
                            }
                        }
                    }

                    meshesMap[cX, cY] = meshData;
                }
            }
            
            
            return meshesMap;
        }

        static Vector3[,] GenerateRandomOffset(int width, int height, Vector2Int xRandRange, Vector2Int yRandRange)
        {
            Vector3[,] offset = new Vector3[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    offset[x, y] = new Vector3(Noise.prng.Next(xRandRange.x, xRandRange.y) / 100f, 0, Noise.prng.Next(yRandRange.x, yRandRange.y) / 100f);
                }
            }

            return offset;
        }
    }

    public class MeshData
    {
        public int width;
        public int height;

        public int[] triangles;
        public Vector3[] vertices;
        public Vector2[] uv;
        public Color[] colors;

        int lastAdd = 0;

        public MeshData(int width, int height)
        {
            this.width = width;
            this.height = height;

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

    public class MeshDataEditor
    {
        MeshData meshData;
        public Vector3[,] verticesMap;

        public MeshDataEditor(MeshData meshData)
        {
            this.meshData = meshData;
            SetVerticesMap();
        }

        void SetVerticesMap()
        {
            verticesMap = new Vector3[meshData.width, meshData.height];

            int vertexIndex = 0;
            for (int x = 0; x < meshData.width - 1; x++)
            {
                for (int y = 0; y < meshData.height - 1; y++)
                {
                    verticesMap[x, y + 1] = meshData.vertices[vertexIndex];
                    verticesMap[x + 1, y] = meshData.vertices[vertexIndex + 1];
                    verticesMap[x, y] = meshData.vertices[vertexIndex + 2];
                    verticesMap[x + 1, y + 1] = meshData.vertices[vertexIndex + 5];
                    vertexIndex += 6;
                }
            }
        }

        void ApplyVerticesMap()
        {
            int vertexIndex = 0;
            for (int x = 0; x < meshData.width - 1; x++)
            {
                for (int y = 0; y < meshData.height - 1; y++)
                {
                    meshData.vertices[vertexIndex] = verticesMap[x, y + 1];
                    meshData.vertices[vertexIndex + 1] = verticesMap[x + 1, y];
                    meshData.vertices[vertexIndex + 2] = verticesMap[x, y];
                    meshData.vertices[vertexIndex + 3] = verticesMap[x + 1, y];
                    meshData.vertices[vertexIndex + 4] = verticesMap[x, y + 1];
                    meshData.vertices[vertexIndex + 5] = verticesMap[x + 1, y + 1];
                    vertexIndex += 6;
                }
            }
        }

        public MeshData GetMeshData()
        {
            ApplyVerticesMap();
            return meshData;
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

