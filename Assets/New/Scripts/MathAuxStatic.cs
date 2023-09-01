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

    }

}