using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class TowerManager : MonoBehaviour
    {
        private const float HIGH_PRECISION_VALUE = .0001f;
        private const float LOW_PRECISION_VALUE = .5f;
        private const int MAIN_TOWER_AI_MODE_INDEX = 0;

        enum RotationType
        {
            Vertical, Horizontal
        }

        public event EventHandler<bool> OnTargetingNotPossibleCheck;

        [Header("Logics")]
        [SerializeField] private List<TowerModeAbstract> m_towerModeList;
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

        private float m_lastTimeChecked;
        private float m_lastTimeCheckedInterval = 1 / 60;
        private Vector3? m_preferablePosition;

        private float m_fireProjectileCooldown = 2f;
        private float m_fireProjectileLastTime = 0f;
        private float m_projectileAcceleration = 100;

        private bool m_canShoot;
        private bool m_isPreciseKill = false;
        private float m_aimPrecision = .5f;

        public void SetParams(float cannonTurningSpeedHorizontal, float cannonTurningSpeedVertical, float projectileAcceleration, float towerFireRate, int activeAIMode)
        {
            m_cannonTurningSpeedHorizontal = cannonTurningSpeedHorizontal;
            m_cannonTurningSpeedVertical = cannonTurningSpeedVertical;
            m_projectileAcceleration = projectileAcceleration;
            m_fireProjectileCooldown = 1 / towerFireRate;

            m_towerModeList[m_currentActiveTowerAIModeID].enabled = false;
            m_currentActiveTowerAIModeID = activeAIMode;
            m_towerModeList[m_currentActiveTowerAIModeID].enabled = true;

            bool activeAIModeIsZero = activeAIMode == MAIN_TOWER_AI_MODE_INDEX;
            m_projectilePrefab.useGravity = activeAIModeIsZero ? false : true;
            m_isPreciseKill = activeAIModeIsZero ? false: true;
            m_aimPrecision = activeAIModeIsZero ? LOW_PRECISION_VALUE : HIGH_PRECISION_VALUE;
        }

        private void Awake()
        {
            m_lastTimeChecked = Time.time;
            foreach (TowerModeAbstract towerMode in m_towerModeList)
            {
                towerMode.SetTargetLayerMask(m_targetLayerMask);
                towerMode.enabled = false;
            }
        }

        private void Update()
        {
            if (m_lastTimeChecked < Time.time - m_lastTimeCheckedInterval)
            {
                m_lastTimeChecked = Time.time;
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
            Rigidbody B = (Rigidbody)Instantiate(m_projectilePrefab, m_shootingPoint.position, m_shootingPoint.rotation);
            B.velocity = m_shootingPoint.forward * m_projectileAcceleration;
            m_fireProjectileLastTime = Time.time;
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
                    MathAuxStatic.SetVectorAxisValue(ref tF, MathAuxStatic.Axis.X, 0f);
                    MathAuxStatic.SetVectorAxisValue(ref fD, MathAuxStatic.Axis.X, 0f);
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
                    dot = tF.y * fD.y + tF.z * fD.z;
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
                        clockwise = -1;
                    if (tF.x > fD.x)
                        clockwise = -clockwise;
                    if (tF.z > fD.z)
                        clockwise = -clockwise;
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
                    speed = m_isPreciseKill && (float)angle * Mathf.Rad2Deg < 1f ? 0.5f : m_cannonTurningSpeedVertical;
                    objectToRotate.Rotate((speed * clockwise * Time.deltaTime), .0f, .0f);
                    break;
                case RotationType.Horizontal:
                    speed = m_isPreciseKill && (float)angle * Mathf.Rad2Deg < 1f ? 0.5f : m_cannonTurningSpeedHorizontal;
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