using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class TowerManager : MonoBehaviour
    {
        [Header("Logics")]
        [SerializeField] private List<TowerModeAbstract> m_towerModeList;

        [SerializeField] private Transform m_shootingPoint;
        [SerializeField] private Rigidbody m_projectilePrefab;
        [SerializeField] private LayerMask m_targetLayerMask;

        [Header("Cannon Model Referencing")]
        [SerializeField] private GameObject m_cannonHorizontalTurning;
        [SerializeField] private GameObject m_cannonVerticalTurning;
        [SerializeField] private GameObject m_targetAim;

        private float m_projectileGravity;
        public float projectileGravity => m_projectileGravity;
        [SerializeField] private float m_projectileAcceleration = 100;
        public float projectileAcceleration => m_projectileAcceleration;
        private float m_cannonTurningSpeedHorizontal;
        public float cannonTurningSpeedHorizontal => m_cannonTurningSpeedHorizontal;
        private float m_cannonTurningSpeedVertical;
        public float cannonTurningSpeedVertical => m_cannonTurningSpeedVertical;
        private int m_currentActiveTowerAIModeID;

        private float m_lastTimeChecked;
        private float m_lastTimeCheckedInterval = 1 / 60;
        private Vector3? m_preferablePosition;

        private float m_fireProjectileCooldown = 2f;
        private float m_fireProjectileLastTime = 0f;
        private bool m_canShoot;

        public void SetParams(float cannonTurningSpeedHorizontal, float cannonTurningSpeedVertical, float projectileAcceleration, float towerFireRate, int activeAIMode)
        {
            m_cannonTurningSpeedHorizontal = cannonTurningSpeedHorizontal;
            m_cannonTurningSpeedVertical = cannonTurningSpeedVertical;
            m_projectileAcceleration = projectileAcceleration;
            m_fireProjectileCooldown = 1 / towerFireRate;

            m_towerModeList[m_currentActiveTowerAIModeID].enabled = false;
            m_currentActiveTowerAIModeID = activeAIMode;
            m_towerModeList[m_currentActiveTowerAIModeID].enabled = true;
            m_projectilePrefab.useGravity = activeAIMode == 0 ? false : true;
        }

        private void Awake()
        {
            m_lastTimeChecked = Time.time;
            foreach (TowerModeAbstract towerMode in m_towerModeList)
            {
                towerMode.SetTargetLayerMask(m_targetLayerMask);
                towerMode.enabled = false;
            }
            //m_currentActiveTowerAIModuleID = 0;
            //m_towerModeList[m_currentActiveTowerAIModuleID].enabled = true;
        }

        private void FixedUpdate()
        {
            if (m_lastTimeChecked < Time.time - m_lastTimeCheckedInterval)
            {
                m_lastTimeChecked = Time.time;
                m_preferablePosition = GetCurrentActiveAIModule().CalculateTargetingPosition(m_shootingPoint.position, projectileAcceleration);
            }
            if (m_preferablePosition != null)
            {
                m_canShoot = true;

                if (RotateObjectTowardsTarget(m_cannonHorizontalTurning.transform, (Vector3)m_preferablePosition, 2))
                {
                    m_canShoot = false;
                }
                if (RotateObjectTowardsTarget(m_cannonVerticalTurning.transform, (Vector3)m_preferablePosition, 1))
                {
                    m_canShoot = false;
                }
                //m_shootingPoint.transform.LookAt((Vector3)m_preferablePosition);

                if (m_canShoot && m_fireProjectileLastTime < Time.time - m_fireProjectileCooldown)
                {
                    Fire();
                }
                m_targetAim.transform.position = (Vector3)m_preferablePosition;
            }

        }

        private void Fire()
        {
            Rigidbody B = (Rigidbody)Instantiate(m_projectilePrefab, m_shootingPoint.position, m_shootingPoint.rotation);
            B.velocity = m_shootingPoint.forward * m_projectileAcceleration;
            m_fireProjectileLastTime = Time.time;
        }

        /// <summary>
        /// Returns 1 if passed true or -1 if passed false.
        /// rotationAxis:
        /// 1 - Vertical,
        /// 2 - Horizintal
        /// </summary>
        /// <param name="objectToRotate">Parameter value to pass.</param>
        /// <returns>Returns an integer based on the passed value.</returns>
        private bool RotateObjectTowardsTarget(Transform objectToRotate, Vector3 targetToRotateTo, int rotationAxis)
        {
            Vector3 tF = objectToRotate.forward;
            Vector3 fD = targetToRotateTo - objectToRotate.position;
            switch (rotationAxis)
            {
                case 1:
                    tF = new Vector3(0f, tF.y, tF.z);
                    fD = new Vector3(0f, fD.y, fD.z);
                    break;
                case 2:
                    tF = new Vector3(tF.x, 0f, tF.z);
                    fD = new Vector3(fD.x, 0f, fD.z);
                    break;
            }

            float dot = -1;
            switch (rotationAxis)
            {
                case 1:
                    dot = tF.y * fD.y + tF.z * fD.z;
                    break;
                case 2:
                    dot = tF.x * fD.x + tF.z * fD.z;
                    break;
            }

            float angle = Mathf.Acos(dot / (tF.magnitude * fD.magnitude));

            if (angle * Mathf.Rad2Deg <= .1f)
            {
                return false;
            }

            //// Output the angle to the console
            //Debug.Log("Angle rad: " + angle);

            //Debug.Log("Angle: " + angle * Mathf.Rad2Deg);
            //// Output Unitys angle
            //Debug.Log("Unity Angle: " + Vector3.Angle(tF, fD));

            // Draw a ray showing the tanks forward facing vector
            Debug.DrawRay(this.transform.position, tF * 10f, Color.green, 2f);
            // Draw a ray showing the vector to the fuel
            Debug.DrawRay(this.transform.position, fD, Color.red, 2f);

            int clockwise = 1;

            switch (rotationAxis)
            {
                case 1:
                    if (MathAuxStatic.CrossProduct(tF, fD).x < .0f)
                        clockwise = -1;
                    if (tF.x > fD.x)
                        clockwise = -clockwise;
                    if (tF.z > fD.z)
                        clockwise = -clockwise;
                    break;
                case 2:
                    if (MathAuxStatic.CrossProduct(tF, fD).y < .0f)
                        clockwise = -1;
                    break;
            }

            float speed;
            switch (rotationAxis)
            {
                case 1:
                    speed = (float)angle * Mathf.Rad2Deg < 5f ? 2f : m_cannonTurningSpeedVertical;
                    objectToRotate.Rotate((speed * clockwise * Time.deltaTime), .0f, .0f);
                    break;
                case 2:
                    speed = (float)angle * Mathf.Rad2Deg < 5f ? 2f : m_cannonTurningSpeedHorizontal;
                    objectToRotate.Rotate(.0f, (speed * clockwise * Time.deltaTime), .0f);
                    break;
            }
            return true;
        }

        public TowerModeAbstract GetCurrentActiveAIModule()
        {
            return m_towerModeList[m_currentActiveTowerAIModeID];
        }
    }

}