using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    [RequireComponent(typeof(BoxCollider))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float m_maxLifeTime = 15;
        [SerializeField] private ParticleSystem m_kaboomEffect;

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
            ActivateKaboom();
        }

        private void ActivateKaboom()
        {
            Instantiate(m_kaboomEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

}