using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class TowerAIModule : MonoBehaviour
    {
        [SerializeField] private string m_modeName;
        [SerializeField] private string m_modeDescription;
        [SerializeField] private TowerModeAbstract m_towerMode;
        public TowerModeAbstract towerMode => m_towerMode;
    }

}