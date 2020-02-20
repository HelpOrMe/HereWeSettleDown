using UnityEngine;

namespace World.Objects.Structure
{
    public interface IPlaceOn
    {
        bool GetPlaceAccess(StructureObject structure, Vector3 position);
    }
}

