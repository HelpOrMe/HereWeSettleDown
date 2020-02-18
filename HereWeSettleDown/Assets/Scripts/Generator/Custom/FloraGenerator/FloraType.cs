using UnityEngine;

namespace Generator.Custom
{
    public class FloraType : MonoBehaviour
    {
        public HeightMaskPattern heightMaskPattern;
        [Range(0, 1)] public float density;
        public FloraObject[] floraObjects;
    }

    [System.Serializable]
    public class FloraObject
    {
        public string name;
        public GameObject prefab;

        public bool invulnerable;
        public bool weak;

        [Range(0, 1)] public float spawnChance;

        public Vector2Int rotationRandomRange;
        public Vector2Int positionRandomRange;
    }
}
