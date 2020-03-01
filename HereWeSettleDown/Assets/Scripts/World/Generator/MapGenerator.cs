// Test

using System.Collections.Generic;
using UnityEngine;
using World.Generator.Helper;
using World.Map;
using Nodes.HeightMapGeneration;
using csDelaunay;

namespace World.Generator
{
    public class MapGenerator : MonoBehaviour
    {
        public int seed = 0;
        public int cellsCount = 124;

        public int worldWidth = 124;
        public int worldHeight = 124;

        public int chunkWidth = 124 / 2;
        public int chunkHeight = 124 / 2;

        public ChunkObject chunkObject;

        public static Voronoi voronoi;
        public static Region[] regions;

        public HeightMapGenerationGraph heightMapGraph;
        private System.Random prng;

        private void Awake()
        {
            prng = new System.Random(seed);
        }

        public void GenerateMap()
        {
            WorldMesh.CreateWorldMesh(worldWidth, worldHeight, chunkWidth, chunkHeight);
            WorldChunkMap.CreateMap(worldWidth, worldHeight, chunkWidth, chunkHeight, chunkObject.transform.localScale);
            WorldChunkMap.CreateChunks(chunkObject, transform, true);

            SetVoronoi();
            CreateRegions();
            ColorCoastline();
        }

        private void SetVoronoi()
        {
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < cellsCount; i++)
            {
                points.Add(new Vector2(prng.Next(0, worldWidth), prng.Next(0, worldHeight)));
            }
            voronoi = new Voronoi(points, new Rectf(0, 0, worldWidth, worldHeight), 2, prng);
        }

        private void ShowVoronoi()
        {
            foreach (LineSegment line in voronoi.VoronoiDiagram())
            {
                DrawLine(RoundPosition(line.p0), RoundPosition(line.p1), Color.red, 999);
            }

            foreach (Vector2 point in voronoi.SiteCoords())
            {
                Color color = Random.ColorHSV();

                DrawHLine(RoundPosition(point), color);
                /*foreach (Vector2 regionPoint in voronoi.Region(point))
                {
                    DrawLine(ScalePosition(regionPoint), ScalePosition(point), color, 999);
                    DrawHLine(ScalePosition(regionPoint), color);
                }*/
            }
        }

        private void ShowConnectedVertices()
        {
            foreach (Vector2 site in voronoi.SiteCoords())
            {
                foreach (LineSegment line in voronoi.VoronoiBoundarayForSite(site))
                {
                    Vector2 prevPos = RoundPosition(line.p1);
                    foreach (Vector2 pos in ConnectPointsByVertices(line.p0, line.p1))
                    {
                        DrawLine(prevPos, pos, Color.black);
                        DrawHLine(prevPos, Color.black);
                        prevPos = pos;
                    }
                }
            }
        }

        private void CreateRegions()
        {
            List<Region> regions = new List<Region>();
            foreach (Vector2 site in voronoi.SiteCoords())
            {
                Vector2Int[] edges = ToVector2Int(voronoi.Region(site).ToArray());
                Vector2Int[] bounds = GetBounds(edges);
                
                regions.Add(new Region(bounds, edges, site));
            }
            MapGenerator.regions = regions.ToArray();
        }

        private void ColorCoastline()
        {
            heightMapGraph.mapWidth = worldWidth;
            heightMapGraph.mapHeight = worldHeight;
            heightMapGraph.setPrng = prng;

            float[,] heightMap = heightMapGraph.requester.GetHeightMap().map;
            foreach (Region region in regions)
            {
                Vector2Int site = new Vector2Int((int)region.site.x, (int)region.site.y);
                if (heightMap[site.x, site.y] >= 1f)
                    region.Fill(Color.yellow);
                else
                    region.Fill(new Color(0.27f, 0.64f, 0.75f));
            }
        }

        private Vector2Int[] GetBounds(Vector2Int[] edges)
        {
            List<Vector2Int> bounds = new List<Vector2Int>();
            for (int i = 0; i < edges.Length; i++)
            {
                int j = (i + 1) % edges.Length;
                Vector2[] connectedPoints = ConnectPointsByVertices(edges[i], edges[j]);
                bounds.AddRange(ToVector2Int(connectedPoints));
            }
            return bounds.ToArray();
        }

        private Vector2[] ConnectPointsByVertices(Vector2 pos0, Vector2 pos1)
        {
            List<Vector2> points = new List<Vector2>();

            Vector2 offset = RoundPosition(pos0 - pos1);
            for (float i = offset.magnitude; i > 0; i--)
            {
                Vector2 point = RoundPosition(Vector2.MoveTowards(pos0, pos1, i));
                if (!points.Contains(point))
                    points.Add(point);
            }
            points.Add(RoundPosition(pos0));
            return points.ToArray();
        }
        
        private void DrawHLine(Vector2 point, Color color, float duration = float.PositiveInfinity)
        {
            Debug.DrawLine(ToVector3(point) + Vector3.down * 4, ToVector3(point) + Vector3.up * 4, color, duration);
        }

        private void DrawLine(Vector2 point1, Vector2 point2, Color color, float duration = float.PositiveInfinity)
        {
            Debug.DrawLine(ToVector3(point1), ToVector3(point2), color, duration);
        }

        private Vector3 ToVector3(Vector2 p)
        {
            return new Vector3(p.x, 0.1f, p.y);
        }

        private Vector2 RoundPosition(Vector2 p)
        {
            return new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
        }

        private Vector2Int ToVector2Int(Vector2 p)
        {
            return new Vector2Int((int)p.x, (int)p.y);
        }

        private Vector2Int[] ToVector2Int(Vector2[] ps)
        {
            Vector2Int[] points = new Vector2Int[ps.Length];
            for (int i = 0; i < ps.Length; i++)
                points[i] = new Vector2Int((int)ps[i].x, (int)ps[i].y);
            return points;
        }
    }
}
