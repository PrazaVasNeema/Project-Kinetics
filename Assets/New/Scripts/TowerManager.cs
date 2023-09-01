using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class TowerManager : MonoBehaviour
    {
        public enum TowerState
        {
            Active,
            Paused,
        }

        public enum CannonState
        {
            Mo,
            Paused,
        }

        [Header("Logics")]
        [SerializeField] private List<TowerAIModule> m_towerAIModuleList;

        [SerializeField] private Transform m_shootingPoint;
        [SerializeField] private Rigidbody m_projectilePrefab;
        [SerializeField] private LayerMask m_targetLayerMask;

        [Header("Cannon Model Referencing")]
        [SerializeField] private GameObject m_cannonHorizontalTurning;
        [SerializeField] private GameObject m_cannonVerticalTurning;

        [SerializeField] private GameObject m_cube;
        [SerializeField] private Rigidbody m_cube2;
        [SerializeField] private float enemySpeed;

        private TowerState m_state;
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
        private float m_lastTimeCheckedInterval = 5 / 60;
        private Vector3 m_preferablePosition = Vector3.zero;

        private float m_fireProjectileCooldown = 2f;
        private float m_fireProjectileLastTime = 0f;

        public void SetParams(float cannonTurningSpeedHorizontal, float cannonTurningSpeedVertical, float projectileAcceleration)
        {
            m_cannonTurningSpeedHorizontal = cannonTurningSpeedHorizontal;
            m_cannonTurningSpeedVertical = cannonTurningSpeedVertical;
            m_projectileAcceleration = projectileAcceleration;
        }

        private void Start()
        {
            m_state = TowerState.Paused;
            m_lastTimeChecked = Time.time;
            foreach (TowerAIModule towerAIModule in m_towerAIModuleList)
            {
                towerAIModule.towerMode.SetTargetLayerMask(m_targetLayerMask);
                towerAIModule.enabled = false;
            }
            Debug.Log(m_towerAIModuleList.Count);
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
            if (!(CalculateAngle(m_cannonHorizontalTurning.transform, m_preferablePosition, 2) || CalculateAngle(m_cannonVerticalTurning.transform, m_preferablePosition, 1)))
            {

                if (m_fireProjectileLastTime < Time.time - m_fireProjectileCooldown)
                {
                    Rigidbody B = (Rigidbody)Instantiate(m_projectilePrefab, m_shootingPoint.position, m_shootingPoint.rotation);
                    B.velocity = m_shootingPoint.forward * m_projectileAcceleration;
                    m_fireProjectileLastTime = Time.time;
                }

            }




            m_cube.transform.position = m_preferablePosition;
            m_cube2.velocity = new Vector3(0f, 0f, enemySpeed * Time.fixedDeltaTime);
        }

        private void Fire()
        {

        }

        /// <summary>
        /// Returns 1 if passed true or -1 if passed false.
        /// rotationAxis:
        /// 0 - all Axis
        /// 1 - no X
        /// 2 - no Y
        /// 3 - no Z
        /// </summary>
        /// <param name="objectToRotate">Parameter value to pass.</param>
        /// <returns>Returns an integer based on the passed value.</returns>
        private bool CalculateAngle(Transform objectToRotate, Vector3 targetToRotateTo, int rotationAxis = 0)
        {
            Vector3 tF = objectToRotate.forward;
            Vector3 fD = targetToRotateTo - objectToRotate.position;
            switch (rotationAxis)
            {
                case 0:
                    break;
                case 1:
                    tF = new Vector3(0, tF.y, tF.z);
                    fD = new Vector3(0, fD.y, fD.z);
                    break;
                case 2:
                    tF = new Vector3(tF.x, 0, tF.z);
                    fD = new Vector3(fD.x, 0, fD.z);
                    break;
                case 3:
                    tF = new Vector3(tF.x, tF.y, 0);
                    fD = new Vector3(fD.x, fD.y, 0);
                    break;
            }

            // Calculate the dot product
            float dot = 1;
            switch (rotationAxis)
            {
                case 0:
                    break;
                case 1:
                    dot = tF.y * fD.y + tF.z * fD.z;
                    break;
                case 2:
                    dot = tF.x * fD.x + tF.z * fD.z;
                    break;
                case 3:
                    dot = tF.x * fD.x + tF.y * fD.y;
                    break;
            }

            float angle = Mathf.Acos(dot / (tF.magnitude * fD.magnitude));

            if (angle * Mathf.Rad2Deg <= .5f)
            {
                //shouldRotate = false;
                return false;
            }

            // Output the angle to the console
            Debug.Log("Angle rad: " + angle);

            Debug.Log("Angle: " + angle * Mathf.Rad2Deg);
            // Output Unitys angle
            Debug.Log("Unity Angle: " + Vector3.Angle(tF, fD));

            // Draw a ray showing the tanks forward facing vector
            Debug.DrawRay(this.transform.position, tF * 10.0f, Color.green, 2.0f);
            // Draw a ray showing the vector to the fuel
            Debug.DrawRay(this.transform.position, fD, Color.red, 2.0f);

            int clockwise = 1;

            // Check the z value of the crossproduct and negate the direction if less than 0

            switch (rotationAxis)
            {
                case 1:
                    if (Cross(tF, fD).x < 0.0f)
                        clockwise = -1;
                    if (tF.x > fD.x)
                        clockwise = -clockwise;
                    if (tF.z > fD.z)
                        clockwise = -clockwise;


                    break;
                case 2:
                    if (Cross(tF, fD).y < 0.0f)
                        clockwise = -1;
                    break;
                case 3:
                    if (Cross(tF, fD).z < 0.0f)
                        clockwise = -1;
                    break;
            }
            // Use Unity to work out the angle for you
            // float unityAngle = Vector3.SignedAngle(tF, fD, this.transform.forward);

            // Get the tank to face the fuel
            // this.transform.Rotate(0.0f, 0.0f, unityAngle);

            // Use our rotation
            //this.transform.Rotate(0.0f, 0.0f, (angle * clockwise * Mathf.Rad2Deg * Time.deltaTime) * 0.02f);
            switch (rotationAxis)
            {
                case 1:
                    objectToRotate.Rotate((m_cannonTurningSpeedHorizontal * clockwise * Time.fixedDeltaTime), 0.0f, .0f);
                    break;
                case 2:
                    objectToRotate.Rotate(0.0f, (m_cannonTurningSpeedHorizontal * clockwise * Time.fixedDeltaTime), .0f);
                    break;
                case 3:
                    objectToRotate.Rotate(0.0f, 0.0f, (m_cannonTurningSpeedHorizontal * clockwise * Time.fixedDeltaTime));
                    break;
            }
            return true;
        }

        Vector3 Cross(Vector3 v, Vector3 w)
        {

            float xMult = v.y * w.z - v.z * w.y;
            float yMult = v.z * w.x - v.x * w.z;
            float zMult = v.x * w.y - v.y * w.x;

            Vector3 crossProd = new Vector3(xMult, yMult, zMult);
            return crossProd;
        }

        public TowerAIModule GetCurrentActiveAIModule()
        {
            return m_towerAIModuleList[m_currentActiveTowerAIModuleID];
        }

        public void SetTowerState(TowerState state)
        {
            m_state = state;
        }

        public bool SetProjectileGravity(float gravity)
        {
            bool gravityIsRelevant = gravity >= 0 ? true : false;
            m_projectileGravity = gravityIsRelevant ? gravity : m_projectileGravity;
            return gravityIsRelevant;
        }

        public bool SetProjectileAcceleration(float acceleration)
        {
            if (acceleration > 0)
            {
                m_projectileAcceleration = acceleration;
                return true;
            }
            else
                return false;
        }

        public bool SetCannonTurningSpeedHorizontal(float speed)
        {
            if (speed > 0)
            {
                m_cannonTurningSpeedHorizontal = speed;
                return true;
            }
            else
                return false;
        }

        public bool SetCannonTurningSpeedVertical(float speed)
        {
            if (speed > 0)
            {
                m_cannonTurningSpeedVertical = speed;
                return true;
            }
            else
                return false;
        }
    }

}