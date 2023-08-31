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
    [SerializeField] private GameObject m_projectilePrefab;
    [SerializeField] private LayerMask m_targetLayerMask;

    [Header("Cannon Model Referencing")]
    [SerializeField] private GameObject m_cannonHorizontalTurning;
    [SerializeField] private GameObject m_cannonVerticalTurning;

    private TowerState m_state;
    private float m_projectileGravity;
    public float projectileGravity => m_projectileGravity;
    private float m_projectileAcceleration;
    public float projectileAcceleration => m_projectileAcceleration;
    private float m_cannonTurningSpeedHorizontal;
    public float cannonTurningSpeedHorizontal => m_cannonTurningSpeedHorizontal;
    private float m_cannonTurningSpeedVertical;
    public float cannonTurningSpeedVertical => m_cannonTurningSpeedVertical;
    private int m_currentActiveTowerAIModuleID;

    private float m_lastTimeChecked;
    private float m_lastTimeCheckedInterval = 60/5;
    private Vector3 m_preferablePosition = Vector3.zero;

    private void Start()
    {
        m_state = TowerState.Paused;
        m_lastTimeChecked = Time.time;
    }

    private void FixedUpdate()
    {
        if (m_lastTimeChecked > Time.time - m_lastTimeCheckedInterval)
        {
            m_preferablePosition = GetCurrentActiveAIModule().towerMode.CalculateTargetingPosition();
        }
        if (m_preferablePosition != m_shootingPoint.forward)
        {

        }
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
