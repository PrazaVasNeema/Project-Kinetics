using System.Collections;
using System.Collections.Generic;
using TestJob;
using UnityEngine;

namespace TestJob
{

    [CreateAssetMenu(fileName = "GameStateSO", menuName = "TestJob/GameStateSO")]
    public class GameStateSO : ScriptableObject
    {
        [SerializeField] private GameStateData m_gameStateData;
        public GameStateData gamestateData => m_gameStateData;

        public void SetGameStateData(float towerTurningSpeedHorizontal, float towerTurningSpeedVertical, float towerProjectileSpeed, float targetSpeedSlider)
        {
            m_gameStateData.towerTurningSpeedHorizontal = towerTurningSpeedHorizontal;
            m_gameStateData.towerTurningSpeedVertical = towerTurningSpeedVertical;
            m_gameStateData.towerProjectileSpeed = towerProjectileSpeed;
            m_gameStateData.targetSpeed = targetSpeedSlider;
        }

        public GameStateData GetGameStateData()
        {
            return m_gameStateData;
        }     
    }

    [System.Serializable]
    public class GameStateData
    {
        public float towerTurningSpeedHorizontal;
        public float towerTurningSpeedVertical;
        public float towerProjectileSpeed;
        public float targetSpeed;

        public GameStateData(float towerTurningSpeedHorizontal, float towerTurningSpeedVertical, float towerProjectileSpeed, float targetSpeed)
        {
            this.towerTurningSpeedHorizontal = towerTurningSpeedHorizontal;
            this.towerTurningSpeedVertical = towerTurningSpeedVertical;
            this.towerProjectileSpeed = towerProjectileSpeed;
            this.targetSpeed = targetSpeed;
        }
    }
}