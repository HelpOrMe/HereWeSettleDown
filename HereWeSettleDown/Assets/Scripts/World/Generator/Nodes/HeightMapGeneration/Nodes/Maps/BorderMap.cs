using UnityEngine;
using XNode;

namespace World.Generator.Nodes.HeightMap.Maps
{
    public class BorderMap : Node
    {
        [Range(0, 1)] public float xDistPercent;
        [Range(0, 1)] public float yDistPercent;

        [Output] public HeightMap outMap;

        public HeightMap GetOutMap()
        {
            HeightMapGenerationGraph ghp = (HeightMapGenerationGraph)graph;
            float[,] outMap = new float[ghp.mapWidth, ghp.mapHeight];

            int xDist = Mathf.RoundToInt(ghp.mapWidth * xDistPercent);
            int yDist = Mathf.RoundToInt(ghp.mapHeight * yDistPercent);

            // Left side
            for (int x = 0; x < xDist; x++)
            {
                for (int y = 0; y < ghp.mapHeight; y++)
                {
                    float fadeValue = (xDist - (x % (ghp.mapWidth / 2))) / (float)xDist;
                    if (outMap[x, y] < fadeValue)
                        outMap[x, y] = fadeValue;
                }
            }

            // Down side
            for (int x = 0; x < ghp.mapWidth; x++)
            {
                for (int y = 0; y < yDist; y++)
                {
                    float fadeValue = (yDist - (y % (ghp.mapHeight / 2))) / (float)yDist;
                    if (outMap[x, y] < fadeValue)
                        outMap[x, y] = fadeValue;
                }
            }

            // Right side
            for (int x = ghp.mapWidth - xDist; x < ghp.mapWidth; x++)
            {
                for (int y = 0; y < ghp.mapHeight; y++)
                {
                    float fadeValue = (x - (ghp.mapWidth - xDist)) / (float)xDist;
                    if (outMap[x, y] < fadeValue)
                        outMap[x, y] = fadeValue;
                }
            }

            // Up side
            for (int x = 0; x < ghp.mapWidth; x++)
            {
                for (int y = ghp.mapHeight - yDist; y < ghp.mapHeight; y++)
                {
                    float fadeValue = (y - (ghp.mapHeight - yDist)) / (float)yDist;
                    if (outMap[x, y] < fadeValue)
                        outMap[x, y] = fadeValue;
                }
            }

            return new HeightMap(outMap);
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "outMap")
                return GetOutMap();
            return null;
        }
    }
}
