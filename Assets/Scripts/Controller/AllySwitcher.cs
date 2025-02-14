using System;
using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Controller
{
    public class AllySwitcher : PlayerComponent
    {
        private PlayerInput _playerInput;
        public int _currentIndex;

        private bool _locked;

        private void Start()
        {
            _playerInput = InputManager.Instance.PlayerInput;

            _playerInput.Combat.Cycle.performed += CycleAlly;
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPlayerBeginMove, Lock);
            EventManager.Subscribe(EventTypes.OnPlayerEndMove, Unlock);
        }
        
        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPlayerBeginMove, Lock);
            EventManager.Unsubscribe(EventTypes.OnPlayerEndMove, Unlock);
        }

        /** Activated by TAB/LSHIFT, switches the active/focused ally */
        private void CycleAlly(InputAction.CallbackContext context)
        {
            if (_locked || CameraController.Locked)
            {
                // Cannot cycle
                // SFX
                // UI
                return;
            }
            
            float inputValue = context.ReadValue<float>();
            _currentIndex = MathUtils.Mod(_currentIndex + (int)inputValue, GameManager.Allies.Count);
            ActiveAllyController.ActiveAlly = GameManager.Allies[_currentIndex];
        }

        private void Lock(object _)
        {
            _locked = true;
        }

        private void Unlock()
        {
            _locked = false;
        }
    }
}