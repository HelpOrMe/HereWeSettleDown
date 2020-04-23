using UnityEngine;
using World.Objects.Structure;

namespace World.Map
{
    public class ChunkObject : MonoBehaviour, IPlaceOn
    {
        public bool GetPlaceAccess(StructureObject structure, Vector3 position)
        {
            return true;
        }
    }
}
