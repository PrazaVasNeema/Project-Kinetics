using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestJob
{

    public class CameraController : MonoBehaviour
    {

        private const int HIGHER_PRIORITY = 20;
        private const int LOWER_PRIORITY = 10;
        private const int MAIN_CAMERA_INDEX = 0;
        private const float MAIN_CAMERA_CHANGE_MULTIPLIER = .15f; 

        [SerializeField] private CinemachineVirtualCamera[] m_virtualCameraArray;

        private int m_currentVirtualCameraIndex;
        private Vector3 m_virtualCameraInitialPositionRelativeToCenter;

        private void Awake()
        {
            m_virtualCameraInitialPositionRelativeToCenter = m_virtualCameraArray[MAIN_CAMERA_INDEX].transform.position;
        }

        public void SetParams(int cameraMode)
        {
            SetCurrentCamera(cameraMode);
        }

        private void SetCurrentCamera(int cameraMode)
        {
            m_virtualCameraArray[m_currentVirtualCameraIndex].Priority = LOWER_PRIORITY;
            m_currentVirtualCameraIndex = cameraMode;
            m_virtualCameraArray[m_currentVirtualCameraIndex].Priority = HIGHER_PRIORITY;
        }

        public void ChangeMainCameraTransform(float multiplier)
        {
            m_virtualCameraArray[MAIN_CAMERA_INDEX].transform.position = MathAuxStatic.CalculateRelativeVectorChange(m_virtualCameraInitialPositionRelativeToCenter, multiplier * MAIN_CAMERA_CHANGE_MULTIPLIER, MathAuxStatic.Axis.None);
        }
    }

}