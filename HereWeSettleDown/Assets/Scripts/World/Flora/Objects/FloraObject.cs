using UnityEngine;
using World.Objects.Structure;

namespace World.Flora.Objects
{
    public class FloraObject : StructureObject
    {
        public bool DestroyOnCollision;
        public int priority;

        public void OnCollisionEnter(Collision collision)
        {
            FloraObject collObj = collision.gameObject.GetComponent<FloraObject>();
            if (collObj && collObj.priority > priority && DestroyOnCollision)
            {
                Destroy(gameObject);
            }
        }
    }
}

