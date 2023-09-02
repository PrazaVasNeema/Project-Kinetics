using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static UnityEditor.SceneView;

namespace TestJob
{

    public class LevelController : MonoBehaviour
    {
        public static LevelController Instance { private set; get; }

        [SerializeField] private UILevelControlPanel m_uiLevelControlPanel;
        [SerializeField] private TowerManager m_towerManager;
        public TowerManager towerManager => m_towerManager;
        [SerializeField] private GameStateSO m_gameStateSO;
        [SerializeField] private WaypointFollower m_waypointFollower;
        [SerializeField] private CameraController m_cameraController;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There is more than one LevelController instance");
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
            m_towerManager.SetParams(gameStateData.towerTurningSpeedHorizontal, gameStateData.towerTurningSpeedVertical, gameStateData.towerProjectileSpeed, gameStateData.towerFireRate, gameStateData.towerAIMode);
            m_uiLevelControlPanel.SetParams(gameStateData);
            m_waypointFollower.SetParams(gameStateData.targetSpeed);

            m_cameraController.SetParams(gameStateData.cameraMode);
            m_cameraController.ChangeMainCameraTransform(gameStateData.targetSpeed);

            m_uiLevelControlPanel.OnGameParamsChanged += M_uiLevelControlPanel_OnGameParamsChanged;
        }

        private void M_uiLevelControlPanel_OnGameParamsChanged(object sender, UILevelControlPanel.OnGameParamsChangedArgs e)
        {
            m_towerManager.SetParams(e.gameStateData.towerTurningSpeedHorizontal, e.gameStateData.towerTurningSpeedVertical, e.gameStateData.towerProjectileSpeed, e.gameStateData.towerFireRate, e.gameStateData.towerAIMode);
            m_waypointFollower.SetParams(e.gameStateData.targetSpeed);

            m_cameraController.SetParams(e.gameStateData.cameraMode);
            m_cameraController.ChangeMainCameraTransform(e.gameStateData.targetSpeed);

            m_waypointFollower.enabled = e.gameStateData.towerAIMode == 0 ? true : false;
        }

    }

}