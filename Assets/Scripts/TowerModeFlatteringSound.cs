using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace TestJob
{

    public class TowerModeFlatteringSound : AbstractTowerMode
    {
        [SerializeField] private bool m_isLow;

        public override Vector3? CalculateTargetingPosition(Vector3 interceptorPos, float interceptorSpeed)
        {
            if (m_target != null)
            {
                float? angle = CalculateAngle(interceptorPos, interceptorSpeed, m_isLow);
                if (angle != null)
                {
                    Vector3 newVector = new Vector3(m_target.transform.position.x, interceptorPos.y, m_target.transform.position.z);
                    Vector3 targetDir = newVector - interceptorPos;
                    Vector3 targetPosition = m_target.position;
                    Vector3 returnVector = new Vector3(targetPosition.x, interceptorPos.y + Mathf.Tan((float)angle * Mathf.Deg2Rad) * targetDir.magnitude, targetPosition.z);
                    return returnVector;
                }
            }
            return null;
        }

        float? CalculateAngle(Vector3 interceptorPos, float interceptorSpeed, bool isLow)
        {
            Vector3 targetDir = m_target.transform.position - interceptorPos;
            float y = targetDir.y;
            targetDir.y = 0.0f;
            float x = targetDir.magnitude;
            float gravity = 9.81f;
            float sSqr = interceptorSpeed * interceptorSpeed;
            float underTheSqrRoot = (sSqr * sSqr) - gravity * (gravity * x * x + 2 * y * sSqr);

            if (underTheSqrRoot >= 0.0f)
            {
                float root = Mathf.Sqrt(underTheSqrRoot);
                float highAngle = sSqr + root;
                float lowAngle = sSqr - root;

                if (isLow) return (Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg);
                else return (Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg);
            }
            else
                return null;
        }
    }

}