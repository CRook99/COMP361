using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using World;

namespace Controller
{
    public struct TargetData
    {
        public Enemy Target;
        public CoverTypes Cover;
    }
    
    public class TargetingSystem : PlayerComponent
    {
        private List<Enemy> _validTargets = new List<Enemy>();
        private bool _aiming = false;
        private int _currentTargetIndex; 
        private PlayerInput _playerInput;
        [SerializeField] private Reticle reticle;

        private void Update()
        {
            if (_aiming)
            {
                HighlightTarget(_validTargets[_currentTargetIndex]);
            }
        }

        private void Start()
        {
            _playerInput = InputManager.Instance.PlayerInput;
            _playerInput.Combat.Cycle.performed += CycleTarget;

            reticle.Hide();
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPlayerBeginAiming, EnterWeaponMode);
            EventManager.Subscribe(EventTypes.OnPlayerConfirmShot, ConfirmShot);
            EventManager.Subscribe(EventTypes.OnPlayerShotFired, ExitWeaponMode);
            EventManager.Subscribe(EventTypes.OnPlayerEndAiming, ExitWeaponMode);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPlayerBeginAiming, EnterWeaponMode);
            EventManager.Unsubscribe(EventTypes.OnPlayerConfirmShot, ConfirmShot);
            EventManager.Unsubscribe(EventTypes.OnPlayerShotFired, ExitWeaponMode);
            EventManager.Unsubscribe(EventTypes.OnPlayerEndAiming, ExitWeaponMode);
        }

        private void EnterWeaponMode()
        {
            if (!TurnManager.Instance.IsAllyTurn() || TurnManager.Instance.HasUnitActed(ActiveAllyController.ActiveAlly)) return; //_modeSwitcher == null
            
            _validTargets = FindValidTargets();
            if (_validTargets.Count == 0)
            {
                // Indicate this
                return;
            }

            ModeSwitcher.SwitchMode(ControlMode.Selection);
            _aiming = true;
            _currentTargetIndex = 0;

            HighlightTarget(_validTargets[_currentTargetIndex]);
        }

        private void ExitWeaponMode()
        {
            ModeSwitcher.SwitchMode(ControlMode.StandardMove);
            _aiming = false;
            reticle.Hide();
        }

        private void CycleTarget(InputAction.CallbackContext context) // TODO: SFX
        {
            if (_validTargets.Count == 0 || !_aiming) return; //!_modeSwitcher

            float inputValue = context.ReadValue<float>();
            int direction = (inputValue > 0) ? -1 : 1;  

            _currentTargetIndex = (_currentTargetIndex + direction + _validTargets.Count) % _validTargets.Count;

            HighlightTarget(_validTargets[_currentTargetIndex]);
        }

        private void HighlightTarget(Enemy target)
        {
            if (_validTargets.Count == 0)
            {
                reticle.Hide();
                return;
            }


            TargetData data = new TargetData()
            {
                Target = target,
                Cover = CoverUtilities.GetImmediateCoverLevel(ActiveAllyController.ActiveAlly.CurrentCell,
                    target.CurrentCell, out _),
            };
            
            Vector3 targetPosition = target.CenterOfMass != null ? target.CenterOfMass.position : target.transform.position;
            reticle.Show();
            reticle.SetPosition(Camera.main.WorldToScreenPoint(targetPosition));
            
            EventManager.TriggerEvent(EventTypes.OnCycleTarget, data);
        }

        private void ConfirmShot()
        {
            if (_validTargets.Count == 0 || !_aiming) return;
            
            EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.Weapon);
            ShotManager.Instance.FireShot(ActiveAllyController.ActiveAlly, _validTargets[_currentTargetIndex]);
            _aiming = false;
            reticle.Hide();
        }

        private List<Enemy> FindValidTargets() // We want to copy GameManager.Enemies so we can remove invalid enemies from the list
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

        private bool IsTargetValid(Enemy enemy) // Works - more rules need to be defined
        {
            if (enemy == null) return false;
            

            // TODO: Weapon range
            return !TacticsGrid.Instance.ObstacleBetweenCells(ActiveAllyController.ActiveAlly.CurrentCell, enemy.CurrentCell);
        }
    }

}
