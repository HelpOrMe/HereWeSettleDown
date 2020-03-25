using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helper.Scene
{
    public static class ObjectMono
    {
        public static MonoBehaviour MonoBeh
        {
            get
            {
                if (monoBeh == null)
                    monoBeh = GetNewMonoBehaviourObject();
                return monoBeh;
            }
        }
        private static MonoBehaviour monoBeh;

        public static MonoBehaviour GetNewMonoBehaviourObject()
        {
            GameObject obj = new GameObject("_ObjectMonoBehScript");
            return obj.AddComponent<ObjectMonoBehaviour>();
        }

        private class ObjectMonoBehaviour : MonoBehaviour { }
    }
}
