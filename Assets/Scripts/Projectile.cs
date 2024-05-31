using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TestJob
{

    [RequireComponent(typeof(BoxCollider))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float m_maxLifeTime = 15;
        [SerializeField] private ParticleSystem m_kaboomEffectHit;
        [SerializeField] private ParticleSystem m_kaboomEffectMiss;
        [SerializeField] private LayerMask m_activationOnExitLayerMask;
        [SerializeField] private LayerMask m_activationOnEnterLayerMask;

        private float m_birthTime;

        private void Awake() 
        {
            m_birthTime = Time.time;
        }

        private void Update()
        {
            if (m_birthTime < Time.time - m_maxLifeTime)
                ActivateKaboom(m_kaboomEffectHit);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((m_activationOnEnterLayerMask.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
            {
                ActivateKaboom(m_kaboomEffectHit);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((m_activationOnExitLayerMask.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            {
                ActivateKaboom(m_kaboomEffectMiss);
            }
        }

        private void ActivateKaboom(ParticleSystem kaboomEffect)
        {
            ParticleSystem particleSystemSpawned = Instantiate(kaboomEffect, transform.position, transform.rotation);
            particleSystemSpawned.transform.SetParent(transform.parent);
            Destroy(gameObject);
        }

        public void SetMaxLifeTime(float maxLifeTime)
        {
            m_maxLifeTime = maxLifeTime;
        }
    }

}