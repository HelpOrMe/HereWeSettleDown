using UnityEngine;

namespace Generator
{
    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(int seed, float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve)
        {
            System.Random prng = new System.Random(seed);

            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (height - 1) / 2f;

            MeshData meshData = new MeshData(width, height);

            int vertexIndex = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x * 2 + prng.Next(0, 100) / 100f, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y * 2 + prng.Next(0, 100) / 100f);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                        meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }
            
            return meshData;
        }
    }
    
    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
        public Color[] colors;

        public int width;
        public int height;

        public ColorPack[] colorPacks;

        int triangleIndex;

        public MeshData(int meshWidth, int meshHeight)
        {
            width = meshWidth;
            height = meshHeight;

            vertices = new Vector3[width * height];
            uvs = new Vector2[width * height];
            triangles = new int[(width - 1) * (height - 1) * 6];

            colorPacks = new ColorPack[width * height];
        }

        public void SetColorMap(Color[] colorMap)
        {
            ColorPack[] newColorMap = new ColorPack[colorMap.Length];
            for (int i = 0; i < colorMap.Length; i++)
                newColorMap[i] = new ColorPack(colorMap[i]);
            SetColorMap(newColorMap);
        }

        public void SetColorMap(ColorPack[] colorMap)
        {
            if (colorMap.Length == colorPacks.Length)
                colorPacks = colorMap;
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public void SetVertex(int x, int y, Vector3 vertex)
        {
            int index = Index(x, y);
            if (index < vertices.Length)
                vertices[index] = vertex;
        }

        public void SetColor(int x, int y, ColorPack color)
        {
            int index = Index(x, y);

            if (index < 0 || index > colorPacks.Length)
                return;
            colorPacks[index] = color;
        }

        public Mesh CreateMesh()
        {
            MeshData newData = Rebuild(this);

            Mesh mesh = new Mesh
            {
                vertices = newData.vertices,
                triangles = newData.triangles,
                uv = newData.uvs,
                colors = newData.colors
            };
            mesh.RecalculateNormals();
            return mesh;
        }

        MeshData Rebuild(MeshData meshData)
        {
            MeshData newMeshdata = new MeshData(width, height);

            Vector3[] newVertices = new Vector3[meshData.triangles.Length];
            Color[] newColors = new Color[meshData.triangles.Length];
            Vector2[] newUV = new Vector2[meshData.triangles.Length];
            int[] newTriangles = new int[meshData.triangles.Length];

            // Rebuild mesh so that every triangle has unique vertices

            ColorPack lastColorPack = new ColorPack();
            int ind = 0;
            for (var i = 0; i < meshData.triangles.Length; i++)
            {
                newVertices[i] = meshData.vertices[meshData.triangles[i]];
                if (i % 3 == 0)
                {
                    newColors[i] = meshData.colorPacks[meshData.triangles[i]][0];
                    lastColorPack = meshData.colorPacks[meshData.triangles[i]];
                }
                else
                    newColors[i] = lastColorPack[ind];
                newUV[i] = meshData.uvs[meshData.triangles[i]];
                newTriangles[i] = i;

                ind++;
                if (ind > 2)
                    ind = 0;
            }

            newMeshdata.vertices = newVertices;
            newMeshdata.colors = newColors;
            newMeshdata.uvs = newUV;
            newMeshdata.triangles = newTriangles;
            return newMeshdata;
        }

        int Index(int x, int y) => width * y + x;
    }

    public struct ColorPack
    {
        public ColorPack(Color color1, Color color2, Color color3)
        {
            this.color1 = color1;
            this.color2 = color2;
            this.color3 = color3;
        }

        public ColorPack(Color color)
        {
            color1 = color;
            color2 = color;
            color3 = color;
        }

        public Color this[int index]
        {
            get
            {
                if (index == 0)
                    return color1;
                else if (index == 1)
                    return color2;
                else
                    return color3;
            }
        }

        public Color color1;
        public Color color2;
        public Color color3;
    }
}