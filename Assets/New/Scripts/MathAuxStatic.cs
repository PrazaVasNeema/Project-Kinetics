using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public static class MathAuxStatic
    {

        public enum Axis
        {
            X, Y, Z, None
        }

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

        public static Vector3 CalculateRelativeVectorChange(Vector3 dir, float multiplier, Axis ignoreAxis)
        {
            Vector3 newVector;
            newVector = dir * multiplier;
            SetVectorAxisValue(ref newVector, ignoreAxis, GetVectorAxisValue(dir, ignoreAxis));
            return newVector;
        }

        public static void SetVectorAxisValue(ref Vector3 vectorToChange, Axis axis, float value)
        {
            switch (axis)
            {
                case Axis.X:
                    vectorToChange = new Vector3(value, vectorToChange.y, vectorToChange.z);
                    break;
                case Axis.Y:
                    vectorToChange = new Vector3(vectorToChange.x, value, vectorToChange.z);
                    break;
                case Axis.Z:
                    vectorToChange = new Vector3(vectorToChange.z, vectorToChange.y, value);
                    break;
            }
        }

        public static float GetVectorAxisValue(Vector3 v,  Axis axis)
        {
            float axisValue = -1;
            switch(axis)
            {
                case Axis.X:
                    axisValue = v.x;
                    break;
                case Axis.Y:
                    axisValue = v.y;
                    break;
                case Axis.Z:
                    axisValue = v.z;
                    break;
            }
            return axisValue;
        }
    }

}