using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class SpawnerNew : MonoBehaviour
    {
        public float m_interval = 3;
        public GameObject m_moveTarget;
        [SerializeField] private GameObject m_target;

        private float m_lastSpawn = -1;

        void Update()
        {
            if (Time.time > m_lastSpawn + m_interval)
            {
                var newMonster = Instantiate(m_target);

                var r = newMonster.AddComponent<Rigidbody>();
                r.useGravity = false;
                newMonster.transform.position = transform.position;
                var monsterBeh = newMonster.AddComponent<Monster>();
                monsterBeh.m_moveTarget = m_moveTarget;

                m_lastSpawn = Time.time;
            }
        }
    }

}