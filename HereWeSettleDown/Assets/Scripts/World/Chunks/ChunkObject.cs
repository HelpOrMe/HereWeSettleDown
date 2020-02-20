using UnityEngine;
using World.Objects.Structure;

public class ChunkObject : MonoBehaviour, IPlaceOn
{
    public bool GetPlaceAccess(StructureObject structure, Vector3 position) => true;
}
