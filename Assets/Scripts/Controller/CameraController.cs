using System.Collections;
using Cinemachine;
using DG.Tweening;
using Controller;
using System;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using World;

public enum CameraMode
{
    Standard,
    AirSupport
}

namespace Controller
{
    public class CameraController : PlayerComponent
    {
        [Header("Cameras")]
        [SerializeField] private Transform cameraSystem;
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private CinemachineVirtualCamera standardVCam;
        [SerializeField] private CinemachineVirtualCamera airSupportVCam;
        private CinemachineTransposer _standardVCamTransposer;
        private CinemachineTransposer _airSupportVCamTransposer;
        private bool _isSwitching;
        private CameraMode _mode;
        private CinemachineTransposer _activeTransposer;
        private CameraConfig _activeConfig;

        [Header("Config")]
        [SerializeField] private CameraConfig standardConfig;
        [SerializeField] private CameraConfig airSupportConfig;
        [SerializeField] private float snapTime;

        [Header("Attachment")]
        [SerializeField] private bool attachOnMove;
        [SerializeField] private float attachedFollowSmoothing; // Higher == closer following

        [Header("Space")]
        [SerializeField] private Ease easeType;
        [SerializeField] private float startingZoom;
        [SerializeField] private float cycleTime;
        private bool _isRotating;
        private float _currentZoom;

        public bool Locked => _isRotating || _isSwitching || brain.IsBlending;
        public CameraMode Mode => _mode;
        public float YRotation => cameraSystem.transform.eulerAngles.y;

        // INPUT
        private PlayerInput _playerInput;
        private InputAction _moveInput;

        private Transform _attachedTransform;
        private bool _attached;

        private Tween _snappingTween;

        private void Awake()
        {
            _standardVCamTransposer = standardVCam.GetCinemachineComponent<CinemachineTransposer>();
            _standardVCamTransposer.m_FollowOffset = standardConfig.Offset;
            _airSupportVCamTransposer = airSupportVCam.GetCinemachineComponent<CinemachineTransposer>();
            _airSupportVCamTransposer.m_FollowOffset = airSupportConfig.Offset;

            _currentZoom = startingZoom;
            _mode = CameraMode.Standard;
        }

        private void Start()
        {
            _playerInput = InputManager.Instance.PlayerInput;

            _moveInput = _playerInput.Combat.MoveCamera;
            _playerInput.Combat.RotateCamera.performed += RotateCamera;
            _playerInput.Combat.SwitchMode.performed += SwitchMode;
            _playerInput.Combat.ZoomCamera.performed += ZoomCamera;

            _playerInput.Combat.Enable();
            
            StartCoroutine(Switch());
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPlayerBeginMove, AttachToTransform);
            EventManager.Subscribe(EventTypes.OnPlayerEndMove, DetachFromTransform);
            EventManager.Subscribe(EventTypes.OnEnemyBeginMove, AttachToTransform);
            EventManager.Subscribe(EventTypes.OnEnemyEndMove, DetachFromTransform);
            EventManager.Subscribe(EventTypes.OnActiveAllyChanged, MoveToPosition);
        }

        private void OnDisable()
        {
            _playerInput.Disable();
            _playerInput.Combat.RotateCamera.performed -= RotateCamera;
            _playerInput.Combat.SwitchMode.performed -= SwitchMode;
            _playerInput.Combat.ZoomCamera.performed -= ZoomCamera;

            EventManager.Unsubscribe(EventTypes.OnPlayerBeginMove, AttachToTransform);
            EventManager.Unsubscribe(EventTypes.OnPlayerEndMove, DetachFromTransform);
            EventManager.Unsubscribe(EventTypes.OnEnemyBeginMove, AttachToTransform);
            EventManager.Unsubscribe(EventTypes.OnEnemyEndMove, DetachFromTransform);
            EventManager.Unsubscribe(EventTypes.OnActiveAllyChanged, MoveToPosition);
        }

        private void Update()
        {
            if (brain.IsBlending) return;
            
            if (_attached)
            {
                cameraSystem.transform.position =
                    Vector3.Lerp(cameraSystem.transform.position, _attachedTransform.position, attachedFollowSmoothing * Time.deltaTime);
            }
            else
            {
                HandleMovement();
                if (_mode == CameraMode.AirSupport) HandleSnapping();
            }
        }

        private void SwitchMode(InputAction.CallbackContext context)
        {
            if (_isRotating || brain.IsBlending || ModeSwitcher.CurrentMode != ControlMode.StandardMove) return;

            _mode = _mode == CameraMode.Standard ? CameraMode.AirSupport : CameraMode.Standard;
            StartCoroutine(Switch());
            EventManager.TriggerEvent(EventTypes.OnCameraModeChanged, _mode);
        }

        private IEnumerator Switch()
        {
            _playerInput.Disable();
            if (_mode == CameraMode.Standard)
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

            yield return new WaitForSeconds(0.5f);
            _playerInput.Enable();
        }

        private void HandleMovement()
        {
            _snappingTween?.Kill();

            // Get relative forward and right vectors of current camera orientation
            var forward = _activeTransposer.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            var right = _activeTransposer.transform.right;
            right.y = 0f;
            right.Normalize();

            Vector2 input = _moveInput.ReadValue<Vector2>();
            cameraSystem.transform.position += (input.y * forward + input.x * right) * (_activeConfig.MovementSpeed * Time.deltaTime);
        }

        private void HandleSnapping()
        {
            Vector2 input = _moveInput.ReadValue<Vector2>();
            if (input.magnitude > 0f) return;

            int roundX = Mathf.RoundToInt(cameraSystem.transform.position.x);
            int roundZ = Mathf.RoundToInt(cameraSystem.transform.position.z);

            if (TacticsGrid.Instance.GetCell(roundX, roundZ) == null) return;

            Vector3 snapPosition = new Vector3(roundX, cameraSystem.transform.position.y, roundZ);

            _snappingTween?.Kill();
            _snappingTween = cameraSystem.transform.DOMove(snapPosition, snapTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _snappingTween = null);
        }

        private void RotateCamera(InputAction.CallbackContext context)
        {
            if (_isRotating || brain.IsBlending) return;

            float inputValue = context.ReadValue<float>();
            float amount = inputValue * -90f; // Q is -1, E is 1. 90f is Q rotation, -90f is E rotation

            _isRotating = true;

            cameraSystem.transform.DORotate(new Vector3(0f, amount, 0f), _activeConfig.RotationTime, RotateMode.LocalAxisAdd)
                .SetEase(easeType)
                .OnComplete(() =>
                {
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

        /** Smoothly moves the camera to look at a given position, locking camera controls along the way */
        public void MoveToPosition(object data)
        {
            if (Locked) return;

            Vector3 position;
            
            // Adapter
            if (data is Vector3 vec)
                position = vec;
            else if (data is Transform tr)
                position = tr.position;
            else if (data is GameObject go)
                position = go.transform.position;
            else if (data is MonoBehaviour mono)
                position = mono.transform.position;
            else
                return;

            cameraSystem.transform.DOMove(position, cycleTime)
                .SetEase(Ease.OutQuad)
                .OnStart(() =>
                {
                    _isSwitching = true;
                })
                .OnComplete(() =>
                {
                    _isSwitching = false;
                });
        }

        public void AttachToTransform(object data)
        {
            if (!attachOnMove) return;

            if (data == null)
            {
                Debug.LogError("Passed null attachTransform to CameraController::AttachToTransform");
                return;
            }

            Transform attachTransform;
            if (data is Transform tr)
                attachTransform = tr;
            else if (data is MonoBehaviour mono)
                attachTransform = mono.transform;
            else
                return;

            _attachedTransform = attachTransform;
            _attached = true; // We use an extra bool since null comparison is expensive
        }

        public void DetachFromTransform()
        {
            if (!attachOnMove) return;

            _attachedTransform = null;
            _attached = false;
        }
    }
}
