using UnityEngine;

namespace Generator
{
    public class Chunk
    {
        public GameObject chunkObject;
        public MeshData meshData;

        public Vector2Int mapPosition;
        public Vector2Int size;

        public bool visible { get; private set; }

        MeshFilter meshFilter;

        public Chunk(GameObject terrain, Transform parent, bool visible, Vector2Int mapPosition, Vector2Int size, MeshData meshData)
        {
            this.size = size;
            this.mapPosition = mapPosition;
            this.meshData = meshData;

            CreateChunkObject(terrain, parent);
            DrawMesh();
            UpdateMeshCollider();
            SetVisible(visible);
        }

        void CreateChunkObject(GameObject terrain, Transform parent)
        {
            chunkObject = Object.Instantiate(terrain, parent);
            chunkObject.name = "Chunk" + mapPosition;

            Vector3 scale = chunkObject.transform.localScale;
            chunkObject.transform.position = new Vector3(mapPosition.x * (size.x - 1) * scale.x, 0, mapPosition.y * (size.y - 1) * scale.z);

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

