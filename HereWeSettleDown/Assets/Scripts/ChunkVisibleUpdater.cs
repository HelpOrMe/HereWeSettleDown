using System.Collections.Generic;
using UnityEngine;

namespace World.Map
{
    public class ChunkVisibleUpdater : MonoBehaviour
    {
        public Transform viewer;
        public int maxViewDistance;

        public List<Chunk> lastLoadedChunks = new List<Chunk>();

        public void Update()
        {
            if (WorldChunkMap.chunkMap != null)
                UpdateVisibleChunks(GetMapViewerPosition());
        }

        public Vector2Int GetMapViewerPosition()
        {
            Vector2Int viewerMapPosition = new Vector2Int(
               Mathf.RoundToInt(viewer.position.x / WorldChunkMap.chunkWidth),
               Mathf.RoundToInt(viewer.position.z / WorldChunkMap.chunkHeight));
            return viewerMapPosition;
        }

        public void UpdateVisibleChunks(Vector2Int viewerPosition)
        {
            // Hide old chunks
            for (int i = 0; i < lastLoadedChunks.Count; i++)
                lastLoadedChunks[i].SetVisible(false);
            lastLoadedChunks.Clear();

            int chunkVisibleDistanceX = Mathf.RoundToInt(maxViewDistance / WorldChunkMap.chunkWidth);
            int chunkVisibleDistanceY = Mathf.RoundToInt(maxViewDistance / WorldChunkMap.chunkHeight);

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
            if (chunkPosition.x >= 0 && chunkPosition.x < WorldChunkMap.worldWidth &&
                chunkPosition.y >= 0 && chunkPosition.y < WorldChunkMap.worldHeight)
            {
                Chunk targetChunk = WorldChunkMap.chunkMap[chunkPosition.x, chunkPosition.y];
                lastLoadedChunks.Add(targetChunk);
                targetChunk.SetVisible(true);
            }
        }
    }
}

