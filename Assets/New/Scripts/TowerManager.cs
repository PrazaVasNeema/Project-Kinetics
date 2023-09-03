using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class TowerManager : MonoBehaviour
    {
        private const float HIGH_PRECISION_VALUE = .00001f;
        private const float LOW_PRECISION_VALUE = .5f;
        private const float PRECISION_KILL_REQUIRED_ANGLE_VALUE = 1f;
        private const float PRECISION_KILL_ANGLE_CHANGE_SPEED_VALUE = .5f;
        private const float PROJECTILE_LIFETIME_SHORT = 10f;
        private const float PROJECTILE_LIFETIME_LONG = 30f;
        private const string PROJECTILE_HOLDER_NAME = "ProjectileHolderLifetime";
        private const int MAIN_TOWER_AI_MODE_INDEX = 0;

        enum RotationType
        {
            Vertical, Horizontal
        }

        private class AngleCheck
        {
            public Queue<float> checkQueue;
            public float turningSpeed;

            public AngleCheck(float firstCheck, float secondCheck, float thirdCheck)
            {
                checkQueue = new Queue<float>();
                checkQueue.Enqueue(firstCheck);
                checkQueue.Enqueue(secondCheck);
                checkQueue.Enqueue(thirdCheck);

            }
        }

            public event EventHandler<bool> OnTargetingNotPossibleCheck;

            [Header("Logics")]
            [SerializeField] private List<AbstractTowerMode> m_towerModeList;
            [SerializeField] private Transform m_shootingPoint;
            [SerializeField] private Rigidbody m_projectilePrefab;
            [SerializeField] private LayerMask m_targetLayerMask;

            [Header("Cannon Model Referencing")]
            [SerializeField] private GameObject m_cannonHorizontalTurning;
            [SerializeField] private GameObject m_cannonVerticalTurning;
            [SerializeField] private GameObject m_targetAim;

            private int m_currentActiveTowerAIModeID;

            private float m_lastTimeCheckedAI;
            private float m_lastTimeCheckedAIInterval = 1 / 30;
            private Vector3? m_preferablePosition;

            private float m_fireProjectileCooldown = 2f;
            private float m_fireProjectileLastTime = 0f;
            private float m_projectileAcceleration = 100;

            private bool m_canShoot;
            private bool m_isPreciseKill = false;
            private float m_aimPrecision = .5f;

            private Dictionary<RotationType, AngleCheck> m_cannonTurningStackCheck = new Dictionary<RotationType, AngleCheck>();
            private GameObject m_projectileHolder;

            public void SetParams(float cannonTurningSpeedHorizontal, float cannonTurningSpeedVertical, float projectileAcceleration, float towerFireRate, int activeAIMode)
            {
                m_cannonTurningStackCheck[RotationType.Vertical].turningSpeed = cannonTurningSpeedVertical;
                m_cannonTurningStackCheck[RotationType.Horizontal].turningSpeed = cannonTurningSpeedHorizontal;
                m_projectileAcceleration = projectileAcceleration;
                m_fireProjectileCooldown = 1 / towerFireRate;

                SetCurrentActiveAIMode(activeAIMode);
            }

            private void Awake()
            {
                m_lastTimeCheckedAI = Time.time;
                foreach (AbstractTowerMode towerMode in m_towerModeList)
                {
                    towerMode.SetTargetLayerMask(m_targetLayerMask);
                    towerMode.enabled = false;
                }
                m_projectileHolder = new GameObject(PROJECTILE_HOLDER_NAME);

                m_cannonTurningStackCheck.Add(RotationType.Vertical, new AngleCheck(-1, 1, 0));
                m_cannonTurningStackCheck.Add(RotationType.Horizontal, new AngleCheck(-1, 1, 0));
            }

            private void FixedUpdate()
            {
                if (m_lastTimeCheckedAI < Time.time - m_lastTimeCheckedAIInterval)
                {
                    m_lastTimeCheckedAI = Time.time;
                    m_preferablePosition = GetCurrentActiveAIModule().CalculateTargetingPosition(m_shootingPoint.position, m_projectileAcceleration);
                }
                if (m_preferablePosition != null)
                {
                    m_canShoot = true;

                    // string test = "";

                    m_canShoot = !RotateObjectTowardsTarget(m_cannonHorizontalTurning.transform, (Vector3)m_preferablePosition, RotationType.Horizontal);
                    // test += "R1";

                    m_canShoot = !RotateObjectTowardsTarget(m_cannonVerticalTurning.transform, (Vector3)m_preferablePosition, RotationType.Vertical);
                    // test += " R2";
                    
                    // if (!String.IsNullOrEmpty(test)) Debug.Log(test);
                    //m_shootingPoint.transform.LookAt((Vector3)m_preferablePosition);

                    if (m_canShoot)
                        if (m_fireProjectileLastTime < Time.time - m_fireProjectileCooldown)
                        {
                            Fire();
                        }
                    m_targetAim.transform.position = (Vector3)m_preferablePosition;
                }
                OnTargetingNotPossibleCheck?.Invoke(this, m_preferablePosition != null);

            }

            private void Fire()
            {
                Rigidbody projectileSpawned = (Rigidbody)Instantiate(m_projectilePrefab, m_shootingPoint.position, m_shootingPoint.rotation);
                projectileSpawned.velocity = m_shootingPoint.forward * m_projectileAcceleration;
                m_fireProjectileLastTime = Time.time;
                projectileSpawned.gameObject.transform.SetParent(m_projectileHolder.transform);
            }

            /// <summary>
            /// Поворачивает objectToRotate в сторону targetToRotateTo по ориентации rotationType
            /// </summary>
            /// <returns>Returns true при изменении rotation</returns>
            private bool RotateObjectTowardsTarget(Transform objectToRotate, Vector3 targetToRotateTo, RotationType rotationType)
            {
                Vector3 tF = objectToRotate.forward;
                Vector3 fD = targetToRotateTo - objectToRotate.position;
                switch (rotationType)
                {
                    case RotationType.Vertical:
                        //MathAuxStatic.SetVectorAxisValue(ref tF, MathAuxStatic.Axis.X, 0f);
                        //MathAuxStatic.SetVectorAxisValue(ref fD, MathAuxStatic.Axis.X, 0f);
                        break;
                    case RotationType.Horizontal:
                        MathAuxStatic.SetVectorAxisValue(ref tF, MathAuxStatic.Axis.Y, 0f);
                        MathAuxStatic.SetVectorAxisValue(ref fD, MathAuxStatic.Axis.Y, 0f);
                        break;
                }

                float dotProduct = -1;
                switch (rotationType)
                {
                    case RotationType.Vertical:
                        dotProduct = tF.x * fD.x + tF.y * fD.y + tF.z * fD.z;
                        break;
                    case RotationType.Horizontal:
                        dotProduct = tF.x * fD.x + tF.z * fD.z;
                        break;
                }

                float angle = Mathf.Acos(dotProduct / (tF.magnitude * fD.magnitude)) * Mathf.Rad2Deg;
                if (m_cannonTurningStackCheck[rotationType].checkQueue.Dequeue() == angle)
                {
                    angle = 0f;
                }
                m_cannonTurningStackCheck[rotationType].checkQueue.Enqueue(angle);
                if (angle <= m_aimPrecision)
                {

                    return false;
                }

                //switch (rotationType)
                //{
                //    case RotationType.Vertical:
                //        Debug.Log($"{RotationType.Vertical}: {angle.ToString("F10")}");
                //        break;
                //    case RotationType.Horizontal:
                //        Debug.Log($"{RotationType.Horizontal}: {angle.ToString("F10")}");
                //        break;
                //}

                int clockwise = 1;
                switch (rotationType)
                {
                    case RotationType.Vertical:
                        if (MathAuxStatic.CrossProduct(tF, fD).x < .0f)
                        {
                            clockwise = -1;
                        }
                        //if (tF.x > fD.x)
                        //    clockwise = -clockwise;
                        if (tF.z > fD.z)
                        {
                            clockwise = -clockwise;

                        }
                        break;
                    case RotationType.Horizontal:
                        if (MathAuxStatic.CrossProduct(tF, fD).y < .0f)
                            clockwise = -1;
                        break;
                }

                float speed = m_isPreciseKill && (float)angle < PRECISION_KILL_REQUIRED_ANGLE_VALUE ? PRECISION_KILL_ANGLE_CHANGE_SPEED_VALUE : m_cannonTurningStackCheck[rotationType].turningSpeed;
                switch (rotationType)
                {
                    case RotationType.Vertical:
                        objectToRotate.Rotate((speed * clockwise * Time.fixedDeltaTime), .0f, .0f);
                        break;
                    case RotationType.Horizontal:
                        objectToRotate.Rotate(.0f, (speed * clockwise * Time.fixedDeltaTime), .0f);
                        break;
                }
                return true;
            }

            public AbstractTowerMode GetCurrentActiveAIModule()
            {
                return m_towerModeList[m_currentActiveTowerAIModeID];
            }

            public void SetCurrentActiveAIMode(int activeAIMode)
            {
                m_towerModeList[m_currentActiveTowerAIModeID].enabled = false;
                m_currentActiveTowerAIModeID = activeAIMode;
                m_towerModeList[m_currentActiveTowerAIModeID].enabled = true;

                bool activeAIModeIsMain = activeAIMode == MAIN_TOWER_AI_MODE_INDEX;
                m_projectilePrefab.useGravity = activeAIModeIsMain ? false : true;
                m_projectilePrefab.GetComponent<Projectile>().SetMaxLifeTime(activeAIModeIsMain ? PROJECTILE_LIFETIME_SHORT : PROJECTILE_LIFETIME_LONG);
                m_isPreciseKill = activeAIModeIsMain ? false : true;
                m_aimPrecision = activeAIModeIsMain ? LOW_PRECISION_VALUE : HIGH_PRECISION_VALUE;
            }
    }
}