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

        private Vector3 m_targetsMovingVector;
        private Vector3[] m_targetsPositions = { Vector3.zero, Vector3.zero };
        private float m_targetsSpeedCheckDelta = .05f;

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

        public Vector3 CalculateInterceptCourse(Vector3 aInterceptorPos, float aInterceptorSpeed)
        {

            Vector3 targetDir = m_target.position - aInterceptorPos;

            float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
            float tSpeed2 = m_targetsMovingVector.sqrMagnitude;
            float fDot1 = Vector3.Dot(targetDir, m_targetsMovingVector);
            float targetDist2 = targetDir.sqrMagnitude;
            float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
            if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
                return Vector3.zero;
            float sqrt = Mathf.Sqrt(d);
            float S1 = (-fDot1 - sqrt) / targetDist2;
            float S2 = (-fDot1 + sqrt) / targetDist2;
            // Debug.Log($"S1: {S1}, S2: {S2}");
            if (S1 < 0.0001f)
            {
                if (S2 < 0.0001f)
                    return Vector3.zero;
                else
                    return (S2) * targetDir + m_targetsMovingVector;
            }
            else if (S2 < 0.0001f)
                return (S1) * targetDir + m_targetsMovingVector;
            else if (S1 < S2)
                return (S2) * targetDir + m_targetsMovingVector;
            else
                return (S1) * targetDir + m_targetsMovingVector;
        }

        public static float FindClosestPointOfApproach(Vector3 aPos1, Vector3 aSpeed1, Vector3 aPos2, Vector3 aSpeed2)
        {
            Vector3 PVec = aPos1 - aPos2;
            Vector3 SVec = aSpeed1 - aSpeed2;
            float d = SVec.sqrMagnitude;
            // if d is 0 then the distance between Pos1 and Pos2 is never changing
            // so there is no point of closest approach... return 0
            // 0 means the closest approach is now!
            if (d >= -0.0001f && d <= 0.0002f)
                return 0.0f;
            return (-Vector3.Dot(PVec, SVec) / d);
        }


        // Start is called before the first frame update
        void Start()
        {
            //m_targetsPositions = new Vector3[2];
            StartCoroutine(CheckTargetsSpeedCo());
        }

        private void FixedUpdate()
        {
            if (m_target != null)
            {
                m_targetsMovingVector = (m_target.position - m_targetsPositions[0]) / Time.fixedDeltaTime;
                m_targetsPositions[0] = m_target.position;
            }
        }

        private IEnumerator CheckTargetsSpeedCo()
        {
            while (true)
            {
                if (m_target != null)
                {
                    //m_targetsPositions[1] = m_target.position;
                    //m_targetsMovingVector = m_targetsPositions[0] - m_targetsPositions[1];
                    //m_targetsMovingVector = cube.velocity;

                    //m_targetsPositions[0] = m_target.position;
                }
                yield return new WaitForSeconds(m_targetsSpeedCheckDelta);
            }
        }
    }

}