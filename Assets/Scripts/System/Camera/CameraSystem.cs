using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public enum CameraMode
{
    Standard,
    AirSupport
}

namespace System.Camera
{
    public class CameraSystem : MonoBehaviour
    {
        [Header("Cameras")] 
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private CinemachineVirtualCamera standardVCam;
        [SerializeField] private CinemachineVirtualCamera airSupportVCam;
        private CinemachineTransposer _standardVCamTransposer;
        private CinemachineTransposer _airSupportVCamTransposer;
        private bool _isSwitching;
        private CameraMode _mode;
        private CinemachineTransposer _activeTransposer;
        private CameraConfig _activeConfig;

        [SerializeField] private CameraConfig standardConfig;
        [SerializeField] private CameraConfig airSupportConfig;
        
        [SerializeField] private Ease easeType;
        [SerializeField] private float startingZoom;
        private bool _isRotating;
        private float _currentZoom;
        
        private void Awake()
        {
            _standardVCamTransposer = standardVCam.GetCinemachineComponent<CinemachineTransposer>();
            _standardVCamTransposer.m_FollowOffset = standardConfig.Offset;
            _airSupportVCamTransposer = airSupportVCam.GetCinemachineComponent<CinemachineTransposer>();
            _airSupportVCamTransposer.m_FollowOffset = airSupportConfig.Offset;

            _currentZoom = startingZoom;
            Switch(CameraMode.Standard);
        }
        
        private void Update()
        {
            if (_isRotating || brain.IsBlending) return;

            HandleSwitch();

            if (brain.IsBlending) return;
            
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }

        private void HandleSwitch()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            
            Switch(_mode == CameraMode.Standard ? CameraMode.AirSupport : CameraMode.Standard);
        }

        private void Switch(CameraMode mode)
        {
            _mode = mode;
            
            if (mode == CameraMode.Standard)
            {
                _activeConfig = standardConfig;
                _activeTransposer = _standardVCamTransposer;
                standardVCam.gameObject.SetActive(true);
                airSupportVCam.gameObject.SetActive(false);
            }
            else
            {
                _activeConfig = airSupportConfig;
                _activeTransposer = _airSupportVCamTransposer;
                standardVCam.gameObject.SetActive(false);
                airSupportVCam.gameObject.SetActive(true);
            }

            // Match zoom between modes
            _currentZoom = Mathf.Clamp(_currentZoom, _activeConfig.MinZoomDistance, _activeConfig.MaxZoomDistance);
            _activeTransposer.m_FollowOffset = _activeConfig.Offset.normalized * _currentZoom;
        }

        private void HandleMovement()
        {
            var forward = _activeTransposer.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            var right = _activeTransposer.transform.right;
            right.y = 0f;
            right.Normalize();


            if (Input.GetKey(KeyCode.W))
            {
                transform.position += forward * (_activeConfig.MovementSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= forward * (_activeConfig.MovementSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= right * (_activeConfig.MovementSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += right * (_activeConfig.MovementSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Rotate(-90f);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Rotate(90f);
            }
        }

        private void Rotate(float amount)
        {
            _isRotating = true;
            
            transform.DORotate(new Vector3(0f, amount, 0f), _activeConfig.RotationTime, RotateMode.LocalAxisAdd)
                .SetEase(easeType)
                .OnComplete(() => {
                    _isRotating = false;
                });
        }

        private void HandleZoom()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput == 0) return;
            
            // Change magnitude of vector from camera to target
            _currentZoom -= scrollInput * _activeConfig.ZoomSpeed;
            _currentZoom = Mathf.Clamp(_currentZoom, _activeConfig.MinZoomDistance, _activeConfig.MaxZoomDistance);
            
            _activeTransposer.m_FollowOffset = _activeConfig.Offset.normalized * _currentZoom;
        }
    }
}
