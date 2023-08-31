using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public enum State
    {
        Active,
        Paused,
    }

    [SerializeField] private List<TowerAIModule> m_towerAIModuleList;
    [SerializeField] private GameObject m_aiModuleMountingSlot;
    [SerializeField] private GameObject m_shootingPoint;
    [Header("Cannon Model Referencing")]
    [SerializeField] private GameObject m_cannonHorizontalTurning;
    [SerializeField] private GameObject m_cannonVerticalTurning;

    private State m_state;

    private void Start()
    {
        m_state = State.Paused;
    }


}
