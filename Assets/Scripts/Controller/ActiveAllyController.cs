using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Controller
{
    public class ActiveAllyController : PlayerComponent
    {
        private Ally _activeAlly;
        private ModeSwitcher _modeSwitcher; // Ask Connor about this!!!! - see #$* - SerializeField??
        private List<Enemy> _validTargets = new List<Enemy>();
        private int _currentTargetIndex = 0; // Good practice to initialise here??

        public Ally ActiveAlly
        {
            get => _activeAlly;
            set
            {
                if (_activeAlly != value)
                {
                    _activeAlly = value;
                    EventManager.TriggerEvent(EventTypes.OnActiveAllyChanged, _activeAlly);
                }
            }
        }

        private IEnumerator Start()
        {
            _modeSwitcher = FindObjectOfType<ModeSwitcher>(); // #%*

            while (GameManager.Allies.Count == 0)
            {
                yield return null;
            }
            
            ActiveAlly = GameManager.Allies[0];
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPlayerBeginAiming, EnterWeaponMode);
            EventManager.Subscribe(EventTypes.OnPlayerConfirmShot, HandleShot);
            InputManager.Instance.PlayerInput.Combat.Cycle.performed += CycleTarget; 
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPlayerBeginAiming, ExitWeaponMode);
            EventManager.Unsubscribe(EventTypes.OnPlayerConfirmShot, HandleShot);
            InputManager.Instance.PlayerInput.Combat.Cycle.performed -= CycleTarget;
        }

        private void EnterWeaponMode()
        {
            if (!TurnManager.Instance.IsAllyTurn() || TurnManager.Instance.HasUnitActed(_activeAlly) || _modeSwitcher == null) return;

            _modeSwitcher.SwitchMode(ActionType.Weapon);
            _validTargets = FindValidTargets();

            SetCurrentTarget();

            EventManager.TriggerEvent(EventTypes.OnPlayerBeginAiming, _activeAlly); // Begin aiming or change mode?
        }

        private void ExitWeaponMode()
        {
            if (!_modeSwitcher) return;

            _modeSwitcher.SwitchMode(ActionType.Move); // Ask Connor
            TargetingUIManager.Instance.HideReticle();
        }

        private void CycleTarget(InputAction.CallbackContext context) // SFX needed??
        {
            if (!_modeSwitcher || _validTargets.Count == 0) return;

            float inputValue = context.ReadValue<float>();
            int direction = (inputValue > 0) ? -1 : 1; // I don't really understand what's going on here, copied CycleAlly 

            _currentTargetIndex = (_currentTargetIndex + direction + _validTargets.Count) % _validTargets.Count;

            SetCurrentTarget();
        }

        private void HighlightTarget(Enemy target)
        {
            TargetingUIManager.Instance.ShowTarget(target);
        } 

        private void HandleShot() // Not yet
        {
            return;
        }

        private List<Enemy> FindValidTargets()
        {
            List<Enemy> targets = new List<Enemy>();

            foreach (Enemy enemy in GameManager.Enemies)
            {
                if (IsTargetValid(enemy))
                {
                    targets.Add(enemy);
                }
            }

            return targets;
        }

        private bool IsTargetValid(Enemy enemy) // TODO
        {
            return false;
        }

        private void SetCurrentTarget()
        {
            if (_validTargets.Count > 0)
            {
                HighlightTarget(_validTargets[_currentTargetIndex]);
            }
            else
            {
                TargetingUIManager.Instance.HideReticle();
            }
        }
    }
}