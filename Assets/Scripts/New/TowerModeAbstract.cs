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
    private Vector3 m_preferablePosition;

    protected abstract void CalculateTargetingPosition();

    protected abstract void FindTarget();

}
