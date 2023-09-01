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
            public float initialY;
        }    

        [SerializeField] private Transform m_waypointsParent;
        [SerializeField] private Rigidbody m_object;
        [SerializeField] private Transform m_centroidVisual;

        private float m_objectSpeed;
        private List<WaypointData> m_waypointDataList;
        private int m_currentWaypointIndex;
        private Vector3 m_centroid;
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
            m_centroid = MathAuxStatic.CalculateCentroid(m_waypointsParent);
            m_centroidVisual.position = m_centroid;
            foreach (WaypointData waypointData in m_waypointDataList)
            {
                Vector3 centroudWithoytY = new Vector3(m_centroid.x, 0f, m_centroid.z);
                waypointData.initialPositionAccordingCentroid = waypointData.transform.position - centroudWithoytY;
                waypointData.initialY = waypointData.transform.position.y;
            }
        }

        private void FixedUpdate()
        {
            if ((m_object.position - m_waypointDataList[m_currentWaypointIndex].transform.position).sqrMagnitude < 1f)
            {
                m_currentWaypointIndex = ++m_currentWaypointIndex % m_waypointDataList.Count == 0 ? 0 : m_currentWaypointIndex;
            }
            float t = Time.fixedDeltaTime * m_objectSpeed;
            m_object.position = Vector3.MoveTowards(m_object.position, m_waypointDataList[m_currentWaypointIndex].transform.position, t);
            //m_object.velocity = (m_waypointDataList[m_currentWaypointIndex].transform.position - m_object.position) * Time.fixedDeltaTime * m_objectSpeed;
        }

        private void SetNewWaypointsPositions()
        {
            foreach(WaypointData waypointData in m_waypointDataList)
            {
                Vector3 newe = waypointData.initialPositionAccordingCentroid* m_objectSpeed *m_speedWeight;
                waypointData.transform.position = new Vector3(newe.x, waypointData.initialY, newe.z);
            }
        }



    }

}