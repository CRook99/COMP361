using System;
using System.Camera;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Input
{
    public class AllySwitcher : MonoBehaviour
    {
        public List<Transform> Transforms;

        private CameraSystem _cameraSystem;
        private PlayerInput _playerInput;
        public int _currentIndex;

        private void Start()
        {
            _playerInput = InputManager.Instance.PlayerInput;

            _playerInput.Combat.Cycle.performed += CycleAlly;

            _cameraSystem = FindObjectOfType<CameraSystem>();
        }


        private void CycleAlly(InputAction.CallbackContext context)
        {
            if (_cameraSystem.Locked)
            {
                // Cannot cycle
                // SFX
                // UI
                return;
            }
            
            float inputValue = context.ReadValue<float>();
            _currentIndex = MathUtils.Mod(_currentIndex + (int)inputValue, Transforms.Count);
            _cameraSystem.MoveToPosition(Transforms[_currentIndex].position);
        }
    }
}