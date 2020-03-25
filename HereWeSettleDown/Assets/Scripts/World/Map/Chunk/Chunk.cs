using UnityEngine;

namespace World.Map
{
    public class Chunk
    {
        public ChunkObject chunkObject;
        public ChunkMesh meshData;

        public bool Visible { get; private set; }

        private MeshFilter meshFilter;

        public Chunk(ChunkObject terrain, Transform parent, bool visible, ChunkMesh meshData)
        {
            this.meshData = meshData;

            CreateChunkObject(terrain, parent);
            DrawMesh();
            UpdateMeshCollider();
            SetVisible(visible);
            meshData.ConnectChunk(this);
        }

        private void CreateChunkObject(ChunkObject terrain, Transform parent)
        {
            chunkObject = Object.Instantiate(terrain, parent);
            chunkObject.name = "Chunk" + meshData.chunkX + " " + meshData.chunkY;
            meshFilter = chunkObject.GetComponent<MeshFilter>();
        }

        private void UpdateMeshCollider()
        {
            MeshCollider collider = chunkObject.GetComponent<MeshCollider>();
            if (!collider)
            {
                collider = chunkObject.gameObject.AddComponent<MeshCollider>();
            }

            if (meshFilter)
            {
                collider.sharedMesh = meshFilter.sharedMesh;
            }
        }

        public void DrawMesh()
        {
            if (meshFilter)
            {
                meshFilter.sharedMesh = meshData.CreateMesh();
            }
        }

        public void SetVisible(bool state)
        {
            if (chunkObject)
            {
                chunkObject.gameObject.SetActive(state);
            }

            Visible = state;
        }
    }
}
