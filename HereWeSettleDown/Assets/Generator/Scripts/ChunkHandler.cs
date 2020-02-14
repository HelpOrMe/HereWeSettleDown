using UnityEngine;

namespace Generator
{
    public class ChunkHandler : MonoBehaviour
    {
        
    }

    public class Chunk
    {
        public GameObject chunkObject;
        public MeshData meshData;

        public Vector2Int mapPosition;
        public Vector2Int size;

        public Chunk(GameObject terrain, Vector2Int mapPosition, Vector2Int size, MeshData meshData)
        {
            this.size = size;
            this.mapPosition = mapPosition;
            this.meshData = meshData;
            CreateChunkObject(terrain);
            DrawMesh();
        }

        void CreateChunkObject(GameObject terrain)
        {
            chunkObject = Object.Instantiate(terrain);
            chunkObject.name = "Chunk";

            Vector3 scale = chunkObject.transform.localScale;
            chunkObject.transform.position = new Vector3(mapPosition.x * (size.x - 1) * scale.x, 0, mapPosition.y * (size.y - 1) * scale.z);
        }

        public void DrawMesh()
        {
            MeshFilter meshFilter = chunkObject.GetComponent<MeshFilter>();
            if (meshFilter)
                meshFilter.sharedMesh = meshData.CreateMesh();
        }

        public void Update()
        {

        }
    }
}

