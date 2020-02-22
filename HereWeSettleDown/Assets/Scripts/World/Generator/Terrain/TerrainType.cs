using UnityEngine;

namespace World.Generator.Terrain
{
    public abstract class TerrainType : MonoBehaviour
    {
        public abstract void OverlayMap(ref float[,] heightMap);
    }
}
