using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class WaypointFollower : MonoBehaviour
    {
        [SerializeField] private Transform m_watpointsParent;
        [SerializeField] private Transform m_object;
        [SerializeField] private Transform m_centroidVisual;

        private float m_objectSpeed;
        private List<Transform> m_waypointsList;
        private int m_currentWaypointIndex;
        private Vector3 m_centroid;
        private float m_speedWeight = .005f;

        public void SetParams(float speed)
        {
            m_objectSpeed = speed;
            SetNewWaypointsPositions();
        }

        private void Awake ()
        {
            m_waypointsList = new List<Transform>();
            m_currentWaypointIndex = 0;
            foreach (Transform waypoint in m_watpointsParent.GetComponentInChildren<Transform>())
            {
                m_waypointsList.Add(waypoint);
            }
            m_centroid = MathAuxStatic.CalculateCentroid(m_watpointsParent);
            m_centroidVisual.position = m_centroid;
        }

        private void Update()
        {
            if ((m_object.position - m_waypointsList[m_currentWaypointIndex].position).sqrMagnitude < 4f)
            {
                m_currentWaypointIndex = ++m_currentWaypointIndex % m_waypointsList.Count == 0 ? 0 : m_currentWaypointIndex;
            }
            float t = Time.deltaTime * m_objectSpeed;
            m_object.position = Vector3.Lerp(transform.position, m_waypointsList[m_currentWaypointIndex].position, t);
        }

        private void SetNewWaypointsPositions()
        {
            foreach(Transform waypoint in m_waypointsList)
            {
                waypoint.position = (waypoint.position - m_centroid) * m_objectSpeed * m_speedWeight;
            }
        }

    }

}