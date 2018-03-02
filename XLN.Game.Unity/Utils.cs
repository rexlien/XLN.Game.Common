using UnityEngine;
using System.Collections;

namespace Game.Common.Unity
{
    public class Utils
    {

        static public GameObject GetChildGameObject(GameObject fromGameObject, string withName)
        {
            //Author: Isaac Dart, June-13.
            Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
            foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
            return null;
        }

        static public GameObject SetLayer(GameObject obj, int layer)
        {
            obj.layer = layer;
            Transform[] children = obj.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                child.gameObject.layer = layer;
            }
            return obj;
        }

        public static void Attach(GameObject parent, GameObject child)
        {
            child.transform.parent = parent.transform;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
        }

    }
}