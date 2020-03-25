using System.Collections.Generic;
using UnityEngine;

namespace World.Objects.Structure
{
    public class StructureObject : MonoBehaviour, IPlaceable, IDestroyable
    {
        protected int maxGetLowPosDist = 5;
        protected int minPlaceAccessDist = 1;
        protected readonly List<Vector3> rayCheckGroundPositions = new List<Vector3>();

        private void Awake()
        {
            SetRayCheckPositions();
        }

        public virtual void SetRayCheckPositions()
        {
            Bounds bounds = GetBounds();
            rayCheckGroundPositions.Clear();

            rayCheckGroundPositions.Add(Vector3.zero);
            for (int x = -1; x < 2; x += 2)
            {
                for (int y = -1; y < 2; y += 2)
                {
                    rayCheckGroundPositions.Add(new Vector3(
                        bounds.size.x * x,
                        bounds.size.y * y));
                }
            }
        }

        public Bounds GetBounds()
        {
            Collider collider = GetComponent<Collider>();
            if (!collider)
            {
                collider = gameObject.AddComponent<BoxCollider>();
            }

            return collider.bounds;
        }

        public virtual bool PlaceObject(Vector3 position)
        {
            if (GetPlaceAccess(position))
            {
                transform.position = position;
                return true;
            }
            return false;
        }

        public virtual Vector3 GetRelevantPosition(Vector3 position)
        {
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.distance < maxGetLowPosDist)
                {
                    return hit.point;
                }
            }
            return position;
        }

        public virtual bool DestroyObject()
        {
            return true;
        }

        protected virtual bool GetPlaceAccess(Vector3 position)
        {
            foreach (Vector3 rayPos in rayCheckGroundPositions)
            {
                if (Physics.Raycast(position + rayPos, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                {
                    Debug.DrawRay(position + rayPos, position + rayPos + Vector3.down * hit.distance, Color.white, 1);
                    if (hit.distance > minPlaceAccessDist)
                    {
                        return false;
                    }

                    IPlaceOn placeOn = hit.collider.gameObject.GetComponent<IPlaceOn>();
                    if (placeOn == null || !placeOn.GetPlaceAccess(this, position))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

