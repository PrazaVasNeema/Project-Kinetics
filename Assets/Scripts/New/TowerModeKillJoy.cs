using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerModeKillJoy : TowerModeAbstract
{

    private Vector3 m_targetsMovingVector;
    private Vector3[] m_targetsPositions = { Vector3.zero, Vector3.zero };
    private float m_targetsSpeedCheckDelta = .05f;

    public override Vector3 CalculateTargetingPosition(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed)
    {
        Vector3 targetDir = aTargetPos - aInterceptorPos;

        float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
        float tSpeed2 = aTargetSpeed.sqrMagnitude;
        float fDot1 = Vector3.Dot(targetDir, aTargetSpeed);
        float targetDist2 = targetDir.sqrMagnitude;
        float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
        if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
            return Vector3.zero;
        float sqrt = Mathf.Sqrt(d);
        float S1 = (-fDot1 - sqrt) / targetDist2;
        float S2 = (-fDot1 + sqrt) / targetDist2;

        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
                return Vector3.zero;
            else
                return (S2) * targetDir + aTargetSpeed;
        }
        else if (S2 < 0.0001f)
            return (S1) * targetDir + aTargetSpeed;
        else if (S1 < S2)
            return (S2) * targetDir + aTargetSpeed;
        else
            return (S1) * targetDir + aTargetSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_targetsPositions = new Vector3[2];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CheckTargetsSpeedCo()
    {
        while (true)
        {
            if (m_target != null)
            {
                m_targetsMovingVector = m_targetsPositions[0] - m_targetsPositions[1];
            }
            yield return new WaitForSeconds(m_targetsSpeedCheckDelta);
        }
    }
}
