using UnityEngine;
using World.Mesh;

namespace World.Chunks
{
    public class Chunk
    {
        public GameObject chunkObject;
        public ChunkMeshData meshData;

        public bool visible { get; private set; }

        MeshFilter meshFilter;

        public Chunk(GameObject terrain, Transform parent, bool visible, ChunkMeshData meshData)
        {
            this.meshData = meshData;

            meshData.SetConnectChunk(this);
            CreateChunkObject(terrain, parent);
            DrawMesh();
            UpdateMeshCollider();
            SetVisible(visible);
        }

        void CreateChunkObject(GameObject terrain, Transform parent)
        {
            chunkObject = Object.Instantiate(terrain, parent);
            chunkObject.name = "Chunk" + meshData.chunkX + " " + meshData.chunkY;

            Vector3 scale = chunkObject.transform.localScale;
            meshFilter = chunkObject.GetComponent<MeshFilter>();
        }

        void UpdateMeshCollider()
        {
            MeshCollider collider = chunkObject.GetComponent<MeshCollider>();
            if (collider)
                Object.Destroy(collider);
            collider = chunkObject.AddComponent<MeshCollider>();
            if (meshFilter)
            {
                collider.sharedMesh = meshFilter.sharedMesh;
            }
        }

        public void DrawMesh()
        {
            if (meshFilter)
                meshFilter.sharedMesh = meshData.CreateMesh();
        }

        public void SetVisible(bool state)
        {
            if (chunkObject)
                chunkObject.SetActive(state);
            visible = state;
        }
    }
}
