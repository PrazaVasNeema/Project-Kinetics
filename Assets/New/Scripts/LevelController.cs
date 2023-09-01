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
        [SerializeField] private WaypointFollower m_waypointFollower;
        [SerializeField] private Transform m_virtualCamera;

        private Vector3 m_virtualCameraInitialPositionAccordingToCenter;
        private float m_speedWeight = .15f;


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
            m_waypointFollower.SetParams(gameStateData.targetSpeed);

            m_virtualCameraInitialPositionAccordingToCenter = m_virtualCamera.position;
            ChangeCameraTransform(gameStateData.targetSpeed);

            m_uiLevelControlPanel.OnGameParamsChanged += M_uiLevelControlPanel_OnGameParamsChanged;
        }

        private void M_uiLevelControlPanel_OnGameParamsChanged(object sender, UILevelControlPanel.OnGameParamsChangedArgs e)
        {
            m_towerManager.SetParams(e.gameStateData.towerTurningSpeedHorizontal, e.gameStateData.towerTurningSpeedVertical, e.gameStateData.towerProjectileSpeed);
            m_waypointFollower.SetParams(e.gameStateData.targetSpeed);

            ChangeCameraTransform(e.gameStateData.targetSpeed);
        }

        private void ChangeCameraTransform(float speed)
        {
            m_virtualCamera.position = m_virtualCameraInitialPositionAccordingToCenter * speed * m_speedWeight;
        }    
    }

}