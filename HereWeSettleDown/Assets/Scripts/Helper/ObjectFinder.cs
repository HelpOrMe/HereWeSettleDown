using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Helper
{
    public static class ObjectFinder
    {
        public static GameObject[] rootGameObjects
        {
            get
            {
                _rootGameObjects = _rootGameObjects ?? SceneManager.GetActiveScene().GetRootGameObjects();
                return _rootGameObjects;
            }
        }
        static GameObject[] _rootGameObjects;

        public static T[] FindRootObjects<T>()
        {
            List<T> objects = new List<T>();
            foreach (GameObject rootGameObject in rootGameObjects)
                objects.AddRange(rootGameObject.GetComponentsInChildren<T>());

            return objects.ToArray();
        }

        public static T FindRootObject<T>()
        {
            foreach (GameObject rootGameObject in rootGameObjects)
            {
                T[] childrenObjects = rootGameObject.GetComponentsInChildren<T>();
                foreach (T childObject in childrenObjects)
                {
                    return childObject;
                }
            }
            return default;
        }
    }
}
