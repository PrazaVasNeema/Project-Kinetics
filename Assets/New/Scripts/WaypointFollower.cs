using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class WaypointFollower : MonoBehaviour
    {

        private class WaypointData
        {
            public Transform transform;
            public Vector3 initialPositionAccordingCentroid;
        }    

        [SerializeField] private Transform m_waypointsParent;
        [SerializeField] private Rigidbody m_objectToMove;
        [SerializeField] private Transform m_centroid;

        private float m_objectSpeed;
        private List<WaypointData> m_waypointDataList;
        private int m_currentWaypointIndex;
        private float m_speedWeight = .1f;

        public void SetParams(float speed)
        {
            m_objectSpeed = speed;
            SetNewWaypointsPositions();
        }

        private void Awake ()
        {
            m_waypointDataList = new List<WaypointData>();
            m_currentWaypointIndex = 0;
            foreach (Transform waypoint in m_waypointsParent.GetComponentInChildren<Transform>())
            {
                WaypointData waypointData = new WaypointData();
                waypointData.transform = waypoint;
                m_waypointDataList.Add(waypointData);
            }
            m_centroid.position = MathAuxStatic.CalculateCentroid(m_waypointsParent);
            foreach (WaypointData waypointData in m_waypointDataList)
            {
                Vector3 centroudWithoytY = new Vector3(m_centroid.position.x, 0f, m_centroid.position.z);
                waypointData.initialPositionAccordingCentroid = waypointData.transform.position - centroudWithoytY;
            }
        }

        private void FixedUpdate()
        {
            if ((m_objectToMove.position - m_waypointDataList[m_currentWaypointIndex].transform.position).sqrMagnitude < 1f)
            {
                m_currentWaypointIndex = ++m_currentWaypointIndex % m_waypointDataList.Count == 0 ? 0 : m_currentWaypointIndex;
            }
            float t = Time.fixedDeltaTime * m_objectSpeed;
            m_objectToMove.position = Vector3.MoveTowards(m_objectToMove.position, m_waypointDataList[m_currentWaypointIndex].transform.position, t);
        }

        private void SetNewWaypointsPositions()
        {
            foreach(WaypointData waypointData in m_waypointDataList)
            {
                waypointData.transform.position = MathAuxStatic.CalculateRelativeVectorChange(waypointData.initialPositionAccordingCentroid, m_objectSpeed * m_speedWeight, MathAuxStatic.Axis.Y);
            }
        }
    }

}