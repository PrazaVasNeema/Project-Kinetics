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
        private const float PRECISION_KILL_ANGLE_CHANGE_SPEED_VALUE = .2f;
        private const float PROJECTILE_LIFETIME_SHORT = 10f;
        private const float PROJECTILE_LIFETIME_LONG = 30f;
        private const string PROJECTILE_HOLDER_NAME = "ProjectileHolderLifetime";
        private const int MAIN_TOWER_AI_MODE_INDEX = 0;

        enum RotationType
        {
            Vertical, Horizontal
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
        
        private float m_cannonTurningSpeedHorizontal;
        private float m_cannonTurningSpeedVertical;
        private int m_currentActiveTowerAIModeID;

        private float m_lastTimeCheckedAI;
        private float m_lastTimeCheckedAIInterval = 1 / 60;
        private Vector3? m_preferablePosition;

        private float m_fireProjectileCooldown = 2f;
        private float m_fireProjectileLastTime = 0f;
        private float m_projectileAcceleration = 100;

        private bool m_canShoot;
        private bool m_isPreciseKill = false;
        private float m_aimPrecision = .5f;

        private GameObject m_projectileHolder;

        public void SetParams(float cannonTurningSpeedHorizontal, float cannonTurningSpeedVertical, float projectileAcceleration, float towerFireRate, int activeAIMode)
        {
            m_cannonTurningSpeedHorizontal = cannonTurningSpeedHorizontal;
            m_cannonTurningSpeedVertical = cannonTurningSpeedVertical;
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
                if (RotateObjectTowardsTarget(m_cannonHorizontalTurning.transform, (Vector3)m_preferablePosition, RotationType.Horizontal))
                {
                    m_canShoot = false;
                }
                if (RotateObjectTowardsTarget(m_cannonVerticalTurning.transform, (Vector3)m_preferablePosition, RotationType.Vertical))
                {
                    m_canShoot = false;
                }
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

            float dot = -1;
            switch (rotationType)
            {
                case RotationType.Vertical:
                    dot = tF.x * fD.x + tF.y * fD.y + tF.z * fD.z;
                    break;
                case RotationType.Horizontal:
                    dot = tF.x * fD.x + tF.z * fD.z;
                    break;
            }

            float angle = Mathf.Acos(dot / (tF.magnitude * fD.magnitude));
            if (angle * Mathf.Rad2Deg <= m_aimPrecision)
            {
                return false;
            }

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

            float speed;
            switch (rotationType)
            {
                case RotationType.Vertical:
                    speed = m_isPreciseKill && (float)angle * Mathf.Rad2Deg < PRECISION_KILL_REQUIRED_ANGLE_VALUE ? PRECISION_KILL_ANGLE_CHANGE_SPEED_VALUE : m_cannonTurningSpeedVertical;
                    objectToRotate.Rotate((speed * clockwise * Time.fixedDeltaTime), .0f, .0f);
                    break;
                case RotationType.Horizontal:
                    speed = m_isPreciseKill && (float)angle * Mathf.Rad2Deg < PRECISION_KILL_REQUIRED_ANGLE_VALUE ? PRECISION_KILL_ANGLE_CHANGE_SPEED_VALUE : m_cannonTurningSpeedHorizontal;
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
            //if (!activeAIModeIsMain)
            //    Time.timeScale = 10f;
        }
    }

}