using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Helper.Scene
{
    public static class ObjectFinder
    {
        public static GameObject[] RootGameObjects
        {
            get
            {
                _rootGameObjects = _rootGameObjects ?? SceneManager.GetActiveScene().GetRootGameObjects();
                return _rootGameObjects;
            }
        }
        static GameObject[] _rootGameObjects;

        public static T[] FindRootSceneObjects<T>()
        {
            List<T> objects = new List<T>();
            foreach (GameObject rootGameObject in RootGameObjects)
                objects.AddRange(rootGameObject.GetComponentsInChildren<T>());

            return objects.ToArray();
        }

        public static T FindRootSceneObject<T>()
        {
            foreach (GameObject rootGameObject in RootGameObjects)
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
