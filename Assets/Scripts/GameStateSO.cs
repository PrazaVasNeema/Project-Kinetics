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
        public float towerFireRate;
        public int cameraMode;
        public int towerAIMode;
        public bool isRandomMovement;

        public GameStateData(float towerTurningSpeedHorizontal, float towerTurningSpeedVertical, float towerProjectileSpeed, float targetSpeed, float towerFireRate, int cameraMode, int towerAIMode, bool isRandomMovement)
        {
            this.towerTurningSpeedHorizontal = towerTurningSpeedHorizontal;
            this.towerTurningSpeedVertical = towerTurningSpeedVertical;
            this.towerProjectileSpeed = towerProjectileSpeed;
            this.targetSpeed = targetSpeed;
            this.towerFireRate = towerFireRate;
            this.cameraMode = cameraMode;
            this.towerAIMode = towerAIMode;
            this.towerAIMode = towerAIMode;
            this.isRandomMovement = isRandomMovement;
        }
    }
}