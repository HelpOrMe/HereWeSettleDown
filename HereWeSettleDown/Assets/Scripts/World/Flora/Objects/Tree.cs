using UnityEngine;

namespace World.Flora.Objects
{
    public class Tree : FloraObject
    {
        public override void SetRayCheckPositions()
        {
            rayCheckGroundPositions.Clear();
            rayCheckGroundPositions.Add(Vector3.zero);
        }
    }
}

