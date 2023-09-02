using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TestJob
{

    public abstract class TowerModeAbstract : MonoBehaviour, IHasMeta
    {
        public enum State
        {
            Targeting,
            TargetLocked,
        }

        [SerializeField] private MetaData m_metaData;

        [SerializeField] private float m_detectionRadius;

        private State m_state;
        protected Transform m_target = null;
        protected LayerMask m_targetLayerMask;

        public virtual void SetTargetLayerMask(LayerMask targetLayerMask)
        {
            m_targetLayerMask = targetLayerMask;
        }

        protected void Update()
        {
            if (Time.frameCount % 5 == 0)
            {
                if (m_target == null)
                {
                    FindTarget();
                }
            }
        }

        public abstract Vector3 CalculateTargetingPosition(Vector3 aInterceptorPos, float aInterceptorSpeed);

        protected virtual void FindTarget()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_detectionRadius, m_targetLayerMask);
            m_target = hitColliders.Length > 0 ? hitColliders[0].transform : null;
        }

        public MetaData GetMeta()
        {
            return m_metaData;
        }
    }

}