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

        private TowerManager m_currentSelectedTower;

        public void OnChangeParamsFun()
        {
            GameStateData gameStateData = new GameStateData(m_towerTurningSpeedHorizontalSlider.value, m_towerTurningSpeedVerticalSlider.value, 
                m_towerProjectileSpeedSlider.value, m_targetSpeedSlider.value, m_towerFireRateSlider.value, m_cameraModeDropdown.value);

            OnGameParamsChanged?.Invoke(this, new OnGameParamsChangedArgs
            {
                gameStateData = gameStateData
                //gameStateData = new GameStateData(m_towerTurningSpeedHorizontalSlider.value, m_towerTurningSpeedVerticalSlider.value,
                //m_towerProjectileSpeedSlider.value, m_targetSpeedSlider.value)
            });
        }

        public void SetParams(GameStateData gameStateData)
        {
            m_towerTurningSpeedHorizontalSlider.value = gameStateData.towerTurningSpeedHorizontal;
            m_towerTurningSpeedVerticalSlider.value = gameStateData.towerTurningSpeedVertical;
            m_towerProjectileSpeedSlider.value = gameStateData.towerProjectileSpeed;
            m_targetSpeedSlider.value = gameStateData.targetSpeed;
            m_towerFireRateSlider.value = gameStateData.towerFireRate;
            m_cameraModeDropdown.value = gameStateData.cameraMode;
        }
    }

}