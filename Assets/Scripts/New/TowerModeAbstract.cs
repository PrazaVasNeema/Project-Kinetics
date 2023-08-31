using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerModeAbstract : MonoBehaviour
{
    public enum State
    {
        Targeting,
        TargetLocked,
    }

    private State m_state;
    protected Transform m_target;

    public abstract Vector3 CalculateTargetingPosition(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed);

    protected virtual void FindTarget()
    {

    }

}
