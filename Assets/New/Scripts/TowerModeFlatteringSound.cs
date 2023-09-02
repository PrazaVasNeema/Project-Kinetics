using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace TestJob
{

    public class TowerModeFlatteringSound : TowerModeAbstract
    {
        [SerializeField] private bool m_isLow;

        public override Vector3? CalculateTargetingPosition(Vector3 aInterceptorPos, float aInterceptorSpeed)
        {
            if (m_target != null)
            {
                float? angle = CalculateAngle(aInterceptorPos, aInterceptorSpeed, m_isLow);
                if (angle != null)
                {
                    Vector3 newVector = new Vector3(m_target.transform.position.x, aInterceptorPos.y, m_target.transform.position.z);
                    Vector3 targetDir = newVector - aInterceptorPos;
                    Vector3 targetPosition = m_target.position;
                    Vector3 returnVector = new Vector3(targetPosition.x, aInterceptorPos.y + Mathf.Tan((float)angle * Mathf.Deg2Rad) * targetDir.magnitude, targetPosition.z);
                    return returnVector;
                }
            }
            return null;
        }

        float? CalculateAngle(Vector3 aInterceptorPos, float aInterceptorSpeed, bool low)
        {
            Vector3 targetDir = m_target.transform.position - aInterceptorPos;
            float y = targetDir.y;
            targetDir.y = 0.0f;
            float x = targetDir.magnitude - 1.0f;
            float gravity = 9.8f;
            float sSqr = aInterceptorSpeed * aInterceptorSpeed;
            float underTheSqrRoot = (sSqr * sSqr) - gravity * (gravity * x * x + 2 * y * sSqr);

            if (underTheSqrRoot >= 0.0f)
            {
                float root = Mathf.Sqrt(underTheSqrRoot);
                float highAngle = sSqr + root;
                float lowAngle = sSqr - root;

                if (low) return (Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg);
                else return (Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg);
            }
            else
                return null;
        }
    }

}