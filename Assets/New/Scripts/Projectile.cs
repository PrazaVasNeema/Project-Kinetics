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
        [SerializeField] private ParticleSystem m_kaboomEffect;
        [SerializeField] private LayerMask m_activationLayerMask;

        private float m_birthTime;

        private void Awake() 
        {
            m_birthTime = Time.time;
        }

        private void Update()
        {
            if (m_birthTime < Time.time - m_maxLifeTime)
                ActivateKaboom();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((m_activationLayerMask.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
            {
                ActivateKaboom();
            }
        }

        private void ActivateKaboom()
        {
            Instantiate(m_kaboomEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

}