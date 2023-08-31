using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private TowerState m_state;
    private float m_projectileGravity;
    public float projectileGravity => m_projectileGravity;
    private float m_projectileAcceleration = 100;
    public float projectileAcceleration => m_projectileAcceleration;
    private float m_cannonTurningSpeedHorizontal;
    public float cannonTurningSpeedHorizontal => m_cannonTurningSpeedHorizontal;
    private float m_cannonTurningSpeedVertical;
    public float cannonTurningSpeedVertical => m_cannonTurningSpeedVertical;
    private int m_currentActiveTowerAIModuleID;

    private float m_lastTimeChecked;
    private float m_lastTimeCheckedInterval = 5/60;
    private Vector3 m_preferablePosition = Vector3.zero;

    private float m_fireProjectileCooldown = 2f;
    private float m_fireProjectileLastTime = 0f;

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
        if (m_preferablePosition != m_shootingPoint.forward)
        {
            m_shootingPoint.LookAt(m_preferablePosition);
            if (m_fireProjectileLastTime < Time.time - m_fireProjectileCooldown)
            {
                Rigidbody B = (Rigidbody)Instantiate(m_projectilePrefab, m_shootingPoint.position, m_shootingPoint.rotation);
                B.velocity = m_shootingPoint.forward * m_projectileAcceleration;
                m_fireProjectileLastTime = Time.time;
            }
        }
        m_cube.transform.position = m_preferablePosition;
    }

    private void Fire()
    {

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
