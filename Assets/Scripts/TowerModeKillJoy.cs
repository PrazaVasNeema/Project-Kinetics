using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TestJob
{

    public class TowerModeKillJoy : AbstractTowerMode
    {
        private Vector3 m_targetsMovingVector;
        private Vector3 m_targetPrevPosition;

        public override Vector3? CalculateTargetingPosition(Vector3 interceptorPos, float interceptorSpeed)
        {
            if (m_target != null)
            {
                Vector3? IC = CalculateInterceptCourse(interceptorPos, interceptorSpeed);
                if (IC != null)
                {
                    Vector3 ICCast = (Vector3)IC;
                    ICCast.Normalize();
                    float interceptionTime1 = FindClosestPointOfApproach(m_target.position, m_targetsMovingVector, interceptorPos, ICCast * interceptorSpeed);
                    Vector3 interceptionPoint = m_target.position + m_targetsMovingVector * interceptionTime1;
                    return interceptionPoint;
                }
            }
            return null;
        }

        public Vector3? CalculateInterceptCourse(Vector3 interceptorPos, float anterceptorSpeed)
        {
            Vector3 targetDir = m_target.position - interceptorPos;
            float interceptorSpeed2 = anterceptorSpeed * anterceptorSpeed;
            float targetSpeed2 = m_targetsMovingVector.sqrMagnitude;
            float fDotProduct1 = Vector3.Dot(targetDir, m_targetsMovingVector);
            float targetDist2 = targetDir.sqrMagnitude;
            float d = (fDotProduct1 * fDotProduct1) - targetDist2 * (targetSpeed2 - interceptorSpeed2);
            if (d < .1f)
                return null;
            float sqrt = Mathf.Sqrt(d);
            float S1 = (-fDotProduct1 - sqrt) / targetDist2;
            float S2 = (-fDotProduct1 + sqrt) / targetDist2;
            if (S1 < .0001f)
            {
                if (S2 < .0001f)
                    return null;
                else
                    return (S2) * targetDir + m_targetsMovingVector;
            }
            else if (S2 < .0001f)
                return (S1) * targetDir + m_targetsMovingVector;
            else if (S1 < S2)
                return (S2) * targetDir + m_targetsMovingVector;
            else
                return (S1) * targetDir + m_targetsMovingVector;
        }

        public static float FindClosestPointOfApproach(Vector3 pos1, Vector3 speed1, Vector3 pos2, Vector3 speed2)
        {
            Vector3 PVec = pos1 - pos2;
            Vector3 SVec = speed1 - speed2;
            float d = SVec.sqrMagnitude;
            if (d >= -0.0001f && d <= 0.0002f)
                return 0.0f;
            return (-Vector3.Dot(PVec, SVec) / d);
        }

        private void FixedUpdate()
        {
            if (m_target != null)
            {
                m_targetsMovingVector = (m_target.position - m_targetPrevPosition) / Time.fixedDeltaTime;
                m_targetPrevPosition = m_target.position;
            }
        }
    }

}