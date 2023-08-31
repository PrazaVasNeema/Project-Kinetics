using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class TowerModeAbstract : MonoBehaviour
{
    public enum State
    {
        Targeting,
        TargetLocked,
    }

    [SerializeField] private float m_detectionRadius;

    private State m_state;
    protected Transform m_target = null;
    protected LayerMask m_targetLayerMask;

    public TowerModeAbstract(LayerMask targetLayerMask)
    {
        m_targetLayerMask = targetLayerMask;
    }

    private void Update()
    {
        if (Time.frameCount % 5 ==0)
        {
            FindTarget();
        }
    }

    public abstract Vector3 CalculateTargetingPosition(Vector3 aInterceptorPos, float aInterceptorSpeed);

    protected virtual void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_detectionRadius, m_targetLayerMask);
        m_target = hitColliders[0].transform;
    }

}
