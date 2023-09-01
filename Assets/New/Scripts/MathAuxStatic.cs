using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public static class MathAuxStatic
    {

        public static Vector3 CalculateCentroid(Transform parentGameObject)
        {
            Vector3 centroid = Vector3.zero;

            if (parentGameObject.childCount > 0)
            {
                Transform[] allChildren = parentGameObject.gameObject.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    centroid += child.transform.position;
                }
                centroid /= (allChildren.Length);
            }

            return centroid;
        }

        public static Vector3 CrossProduct(Vector3 v, Vector3 w)
        {

            float xMult = v.y * w.z - v.z * w.y;
            float yMult = v.z * w.x - v.x * w.z;
            float zMult = v.x * w.y - v.y * w.x;

            Vector3 crossProd = new Vector3(xMult, yMult, zMult);
            return crossProd;
        }

    }

}