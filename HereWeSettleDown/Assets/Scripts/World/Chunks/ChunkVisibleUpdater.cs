using System.Collections.Generic;
using UnityEngine;

namespace World.Chunks
{
    public class ChunkVisibleUpdater : MonoBehaviour
    {
        public Transform viewer;
        public int maxViewDistance;

        public List<Chunk> lastLoadedChunks = new List<Chunk>();

        public void Update()
        {
            if (ChunkMap.chunkMap != null)
                UpdateVisibleChunks(GetMapViewerPosition());
        }

        public Vector2Int GetMapViewerPosition()
        {
            Vector2Int viewerMapPosition = new Vector2Int(
               Mathf.RoundToInt(viewer.position.x / ChunkMap.WorldChunkWidth),
               Mathf.RoundToInt(viewer.position.z / ChunkMap.WorldChunkHeight));
            return viewerMapPosition;
        }

        public void UpdateVisibleChunks(Vector2Int viewerPosition)
        {
            // Hide old chunks
            for (int i = 0; i < lastLoadedChunks.Count; i++)
                lastLoadedChunks[i].SetVisible(false);
            lastLoadedChunks.Clear();

            int chunkVisibleDistanceX = Mathf.RoundToInt(maxViewDistance / ChunkMap.ChunkWidth);
            int chunkVisibleDistanceY = Mathf.RoundToInt(maxViewDistance / ChunkMap.ChunkHeight);

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
            int chunkMapWidth = ChunkMap.chunkMap.GetLength(0);
            int chunkMapHeight = ChunkMap.chunkMap.GetLength(1);

            if (chunkPosition.x >= 0 && chunkPosition.y >= 0 &&
                chunkPosition.x < chunkMapWidth && chunkPosition.y < chunkMapHeight)
            {
                Chunk targetChunk = ChunkMap.chunkMap[chunkPosition.x, chunkPosition.y];
                lastLoadedChunks.Add(targetChunk);
                targetChunk.SetVisible(true);
            }
        }
    }
}

