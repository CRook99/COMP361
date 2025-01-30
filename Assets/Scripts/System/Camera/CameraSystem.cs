using System.Collections;
using Cinemachine;
using DG.Tweening;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private float cycleTime;
        private bool _isRotating;
        private float _currentZoom;

        public bool Locked => _isRotating || _isSwitching || brain.IsBlending;
        
        // INPUT
        private PlayerInput _playerInput;
        private InputAction _moveInput;
        
        private void Awake()
        {
            _standardVCamTransposer = standardVCam.GetCinemachineComponent<CinemachineTransposer>();
            _standardVCamTransposer.m_FollowOffset = standardConfig.Offset;
            _airSupportVCamTransposer = airSupportVCam.GetCinemachineComponent<CinemachineTransposer>();
            _airSupportVCamTransposer.m_FollowOffset = airSupportConfig.Offset;

            _currentZoom = startingZoom;
            Switch(CameraMode.Standard);
        }

        private void OnEnable()
        {
            _playerInput = InputManager.Instance.PlayerInput;
            
            _moveInput = _playerInput.Combat.MoveCamera;
            _playerInput.Combat.RotateCamera.performed += RotateCamera;
            _playerInput.Combat.SwitchMode.performed += SwitchMode;
            _playerInput.Combat.ZoomCamera.performed += ZoomCamera;
            
            _playerInput.Combat.Enable();
        }
        
        private void OnDisable()
        {
            _playerInput.Disable();
            _playerInput.Combat.RotateCamera.performed -= RotateCamera;
            _playerInput.Combat.SwitchMode.performed -= SwitchMode;
            _playerInput.Combat.ZoomCamera.performed -= ZoomCamera;
        }

        private void Update()
        {
            if (brain.IsBlending) return;
            
            HandleMovement();
        }

        private void SwitchMode(InputAction.CallbackContext context)
        {
            if (_isRotating || brain.IsBlending) return;
            
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
            // Get relative forward and right vectors of current camera orientation
            var forward = _activeTransposer.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            var right = _activeTransposer.transform.right;
            right.y = 0f;
            right.Normalize();
            
            Vector2 input = _moveInput.ReadValue<Vector2>();
            transform.position += (input.y * forward + input.x * right) * (_activeConfig.MovementSpeed * Time.deltaTime);
        }
        
        private void RotateCamera(InputAction.CallbackContext context)
        {
            if (_isRotating || brain.IsBlending) return;
            
            float inputValue = context.ReadValue<float>();
            float amount = inputValue * -90f; // Q is -1, E is 1. 90f is Q rotation, -90f is E rotation
            
            _isRotating = true;
            
            transform.DORotate(new Vector3(0f, amount, 0f), _activeConfig.RotationTime, RotateMode.LocalAxisAdd)
                .SetEase(easeType)
                .OnComplete(() => {
                    _isRotating = false;
                });
        }
        
        private void ZoomCamera(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<Vector2>().y;
            _currentZoom -= Mathf.Sign(value) * _activeConfig.ZoomSpeed;
            _currentZoom = Mathf.Clamp(_currentZoom, _activeConfig.MinZoomDistance, _activeConfig.MaxZoomDistance);
            
            _activeTransposer.m_FollowOffset = _activeConfig.Offset.normalized * _currentZoom;
        }

        public void MoveToPosition(Vector3 position)
        {
            if (Locked) return;
            Debug.Log("go");
            
            Vector3 start = transform.position;
            Vector3 end = position;
            
            transform.DOMove(position, cycleTime)
                .SetEase(Ease.OutQuad)
                .OnStart(() => _isSwitching = true)
                .OnComplete(() => _isSwitching = false);
        }
    }
}
