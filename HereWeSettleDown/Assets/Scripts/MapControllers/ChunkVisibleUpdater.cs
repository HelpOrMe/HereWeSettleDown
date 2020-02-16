using System.Collections.Generic;
using UnityEngine;
using Generator;
using Generator.Custom;

namespace Chunks
{
    /*public class ChunkVisibleUpdater : MonoBehaviour
    {
        public Transform viewer;
        public int maxViewDistance;

        List<Chunk> lastLoadedChunks = new List<Chunk>();

        Chunk[,] chunkMap
        {
            get
            {
                return SubGenerator.GetValue<Chunk[,]>("chunkMap");
            }
        }

        public void Update()
        {
            if (chunkMap != null)
                UpdateVisibleChunks(GetMapViewerPosition());
        }

        public Vector2Int GetMapViewerPosition()
        {
            Vector2Int viewerMapPosition = new Vector2Int(
               Mathf.RoundToInt(viewer.position.x / SubGenerator.GetValue<int>("chunkWidth")),
               Mathf.RoundToInt(viewer.position.z / SubGenerator.GetValue<int>("chunkHeight")));
            return viewerMapPosition;
        }

        public void UpdateVisibleChunks(Vector2Int viewerPosition)
        {
            // Hide old chunks
            for (int i = 0; i < lastLoadedChunks.Count; i++)
                lastLoadedChunks[i].SetVisible(false);
            lastLoadedChunks.Clear();

            int chunkVisibleDistanceX = Mathf.RoundToInt(maxViewDistance / SubGenerator.GetValue<int>("chunkWidth"));
            int chunkVisibleDistanceY = Mathf.RoundToInt(maxViewDistance / SubGenerator.GetValue<int>("chunkHeight"));

            for (int xOffset = -chunkVisibleDistanceX; xOffset <= chunkVisibleDistanceX; xOffset++)
            {
                for (int yOffset = -chunkVisibleDistanceY; yOffset <= chunkVisibleDistanceY; yOffset++)
                {
                    Vector2Int chunkPosition = new Vector2Int(viewerPosition.x + xOffset, viewerPosition.y + yOffset);
                    UpdateVisibleChunk(chunkPosition);
                }
            }
        }

        public void UpdateVisibleChunk(Vector2Int chunkPosition)
        {
            int chunkMapWidth = MapGenerator.chunkMap.GetLength(0);
            int chunkMapHeight = MapGenerator.chunkMap.GetLength(1);

            if (chunkPosition.x >= 0 && chunkPosition.y >= 0 &&
                chunkPosition.x < chunkMapWidth && chunkPosition.y < chunkMapHeight)
            {
                Chunk targetChunk = MapGenerator.chunkMap[chunkPosition.x, chunkPosition.y];
                lastLoadedChunks.Add(targetChunk);
                targetChunk.SetVisible(true);
            }
        }
    }*/
}

