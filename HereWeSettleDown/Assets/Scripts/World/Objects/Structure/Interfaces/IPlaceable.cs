using UnityEngine;

namespace World.Objects.Structure
{
    public interface IPlaceable
    {
        bool PlaceObject(Vector3 position);

        Vector3 GetRelevantPosition(Vector3 position);
    }
}