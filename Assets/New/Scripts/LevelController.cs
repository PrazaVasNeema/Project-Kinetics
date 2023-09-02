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
        // [SerializeField] private TowerManager m_target;
        [SerializeField] private GameStateSO m_gameStateSO;
        public GameStateSO gameStateSO => m_gameStateSO;
        [SerializeField] private WaypointFollower m_waypointFollower;
        [SerializeField] private CinemachineVirtualCamera[] m_virtualCameraArray;

        private int m_currentVirtualCameraIndex;
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
            m_towerManager.SetParams(gameStateData.towerTurningSpeedHorizontal, gameStateData.towerTurningSpeedVertical, gameStateData.towerProjectileSpeed, gameStateData.towerFireRate, gameStateData.towerAIMode);
            m_uiLevelControlPanel.SetParams(gameStateData);
            m_waypointFollower.SetParams(gameStateData.targetSpeed);

            m_currentVirtualCameraIndex = gameStateData.cameraMode;
            m_virtualCameraInitialPositionAccordingToCenter = m_virtualCameraArray[m_currentVirtualCameraIndex].transform.position;
            ChangeCameraTransform(gameStateData.targetSpeed);
            SetCurrentCamera(gameStateData.cameraMode);

            m_uiLevelControlPanel.OnGameParamsChanged += M_uiLevelControlPanel_OnGameParamsChanged;
        }

        private void M_uiLevelControlPanel_OnGameParamsChanged(object sender, UILevelControlPanel.OnGameParamsChangedArgs e)
        {
            m_towerManager.SetParams(e.gameStateData.towerTurningSpeedHorizontal, e.gameStateData.towerTurningSpeedVertical, e.gameStateData.towerProjectileSpeed, e.gameStateData.towerFireRate, e.gameStateData.towerAIMode);
            m_waypointFollower.SetParams(e.gameStateData.targetSpeed);

            ChangeCameraTransform(e.gameStateData.targetSpeed);
            SetCurrentCamera(e.gameStateData.cameraMode);

            m_waypointFollower.enabled = e.gameStateData.towerAIMode == 0 ? true : false;
        }

        private void ChangeCameraTransform(float speed)
        {
            m_virtualCameraArray[m_currentVirtualCameraIndex].transform.position = m_virtualCameraInitialPositionAccordingToCenter * speed * m_speedWeight;
        }    

        private void SetCurrentCamera(int cameraMode)
        {
            m_virtualCameraArray[m_currentVirtualCameraIndex].Priority = 10;
            m_currentVirtualCameraIndex = cameraMode;
            m_virtualCameraArray[m_currentVirtualCameraIndex].Priority = 20;
        }
    }

}