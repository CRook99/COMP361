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
            EventManager.Subscribe(EventTypes.OnPlayerBeginAiming, Lock);
            EventManager.Subscribe(EventTypes.OnPlayerEndAiming, Unlock);
            EventManager.Subscribe(EventTypes.OnPlayerBeginAbility, Lock);
            EventManager.Subscribe(EventTypes.OnPlayerEndAbility, Unlock);
        }
        
        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPlayerBeginMove, Lock);
            EventManager.Unsubscribe(EventTypes.OnPlayerEndMove, Unlock);
            EventManager.Unsubscribe(EventTypes.OnPlayerBeginAiming, Lock);
            EventManager.Unsubscribe(EventTypes.OnPlayerEndAiming, Unlock);
            EventManager.Unsubscribe(EventTypes.OnPlayerBeginAbility, Lock);
            EventManager.Unsubscribe(EventTypes.OnPlayerEndAbility, Unlock);
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
            
            HintManager.Instance.FulfilTutorial(TutorialSteps.Allies);
        }

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock()
        {
            _locked = false;
        }
    }
}