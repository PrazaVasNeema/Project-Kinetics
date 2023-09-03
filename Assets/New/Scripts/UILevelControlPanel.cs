using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TestJob
{

    public class UILevelControlPanel : MonoBehaviour
    {
        public event EventHandler<OnGameParamsChangedArgs> OnGameParamsChanged;
        public class OnGameParamsChangedArgs : EventArgs
        {
            public GameStateData gameStateData;
        }

        [SerializeField] private Slider m_towerTurningSpeedHorizontalSlider;
        [SerializeField] private Slider m_towerTurningSpeedVerticalSlider;
        [SerializeField] private Slider m_towerProjectileSpeedSlider;
        [SerializeField] private Slider m_targetSpeedSlider;
        [SerializeField] private Slider m_towerFireRateSlider;
        [SerializeField] private TMP_Dropdown m_cameraModeDropdown;
        [SerializeField] private TMP_Dropdown m_towerAIModeDropdown;
        [SerializeField] private TMP_Text m_towerAIModeText;
        [SerializeField] private string[] m_towerAIModeDescritpions;
        [SerializeField] private GameObject m_cantShootAlert;
        [SerializeField] private Toggle m_tenXSpeedToogle;

        private void Start()
        {
            LevelController.Instance.towerManager.OnTargetingNotPossibleCheck += TowerManager_OnCantShoot;
        }

        private void TowerManager_OnCantShoot(object sender, bool e)
        {
            m_cantShootAlert.SetActive(!e);
        }

        public void OnChangeParamsFun()
        {
            GameStateData gameStateData = new GameStateData(m_towerTurningSpeedHorizontalSlider.value, m_towerTurningSpeedVerticalSlider.value, 
                m_towerProjectileSpeedSlider.value, m_targetSpeedSlider.value, m_towerFireRateSlider.value, m_cameraModeDropdown.value, m_towerAIModeDropdown.value);
            m_towerAIModeText.text = m_towerAIModeDescritpions[gameStateData.towerAIMode];

            OnGameParamsChanged?.Invoke(this, new OnGameParamsChangedArgs
            {
                gameStateData = gameStateData
            });
            Time.timeScale = m_tenXSpeedToogle.isOn ? 10f : 1f;
        }

        public void SetParams(GameStateData gameStateData)
        {
            m_towerTurningSpeedHorizontalSlider.value = gameStateData.towerTurningSpeedHorizontal;
            m_towerTurningSpeedVerticalSlider.value = gameStateData.towerTurningSpeedVertical;
            m_towerProjectileSpeedSlider.value = gameStateData.towerProjectileSpeed;
            m_targetSpeedSlider.value = gameStateData.targetSpeed;
            m_towerFireRateSlider.value = gameStateData.towerFireRate;
            m_cameraModeDropdown.value = gameStateData.cameraMode;
            m_towerAIModeDropdown.value = gameStateData.towerAIMode;
            m_towerAIModeText.text = m_towerAIModeDescritpions[gameStateData.towerAIMode];
        }
    }

}