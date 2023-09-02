using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class TowerManager : MonoBehaviour
    {
        [Header("Logics")]
        [SerializeField] private List<TowerAIModule> m_towerAIModuleList;

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
        private int m_currentActiveTowerAIModuleID;

        private float m_lastTimeChecked;
        private float m_lastTimeCheckedInterval = 1 / 60;
        private Vector3 m_preferablePosition = Vector3.zero;

        private float m_fireProjectileCooldown = 2f;
        private float m_fireProjectileLastTime = 0f;

        public void SetParams(float cannonTurningSpeedHorizontal, float cannonTurningSpeedVertical, float projectileAcceleration, float towerFireRate)
        {
            m_cannonTurningSpeedHorizontal = cannonTurningSpeedHorizontal;
            m_cannonTurningSpeedVertical = cannonTurningSpeedVertical;
            m_projectileAcceleration = projectileAcceleration;
            m_fireProjectileCooldown = 1 / towerFireRate;
        }

        private void Start()
        {
            m_lastTimeChecked = Time.time;
            foreach (TowerAIModule towerAIModule in m_towerAIModuleList)
            {
                towerAIModule.towerMode.SetTargetLayerMask(m_targetLayerMask);
                towerAIModule.enabled = false;
            }
            m_currentActiveTowerAIModuleID = 0;
            m_towerAIModuleList[m_currentActiveTowerAIModuleID].enabled = true;
        }

        private void FixedUpdate()
        {
            if (m_lastTimeChecked < Time.time - m_lastTimeCheckedInterval)
            {
                m_lastTimeChecked = Time.time;
                m_preferablePosition = GetCurrentActiveAIModule().towerMode.CalculateTargetingPosition(m_shootingPoint.position, projectileAcceleration);
            }
            if (!(RotateObjectTowardsTarget(m_cannonHorizontalTurning.transform, m_preferablePosition, 2) || RotateObjectTowardsTarget(m_cannonVerticalTurning.transform, m_preferablePosition, 1)))
            {
                if (m_fireProjectileLastTime < Time.time - m_fireProjectileCooldown)
                {
                    Fire();
                }
            }
            m_targetAim.transform.position = m_preferablePosition;
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

            if (angle * Mathf.Rad2Deg <= .5f)
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

            switch (rotationAxis)
            {
                case 1:
                    objectToRotate.Rotate((m_cannonTurningSpeedHorizontal * clockwise * Time.fixedDeltaTime), .0f, .0f);
                    break;
                case 2:
                    objectToRotate.Rotate(.0f, (m_cannonTurningSpeedHorizontal * clockwise * Time.fixedDeltaTime), .0f);
                    break;
            }
            return true;
        }

        public TowerAIModule GetCurrentActiveAIModule()
        {
            return m_towerAIModuleList[m_currentActiveTowerAIModuleID];
        }
    }

}