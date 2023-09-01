using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class LevelController : MonoBehaviour
    {
        public static LevelController Instance { private set; get; }

        [SerializeField] private UILevelControlPanel m_uiLevelControlPanel;
        [SerializeField] private TowerManager m_towerManager;
        // [SerializeField] private TowerManager m_target;
        [SerializeField] private GameStateSO m_gameStateSO;
        public GameStateSO gameStateSO => m_gameStateSO;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There is more than one player instance");
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Start()
        {
            GameStateData gameStateData = m_gameStateSO.GetGameStateData();
            m_towerManager.SetParams(gameStateData.towerTurningSpeedHorizontal, gameStateData.towerTurningSpeedVertical, gameStateData.towerProjectileSpeed);
            m_uiLevelControlPanel.SetParams(gameStateData);
            m_uiLevelControlPanel.OnGameParamsChanged += M_uiLevelControlPanel_OnGameParamsChanged;
        }

        private void M_uiLevelControlPanel_OnGameParamsChanged(object sender, UILevelControlPanel.OnGameParamsChangedArgs e)
        {
            m_towerManager.SetParams(e.gameStateData.towerTurningSpeedHorizontal, e.gameStateData.towerTurningSpeedVertical, e.gameStateData.towerProjectileSpeed);
        }
    }

}