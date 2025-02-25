using System.Collections;
using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class TargetingSystem : PlayerComponent
    {
        private ModeSwitcher _modeSwitcher; // Ask Connor about this!!!! - see #$*
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

            if (Input.GetKeyDown(KeyCode.H))
            {
                EventManager.TriggerEvent(EventTypes.OnPlayerBeginAiming);
                _aiming = true;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                EventManager.TriggerEvent(EventTypes.OnPlayerEndAiming);
                _aiming = false;
            }
        }

        private void Start()
        {
            _playerInput = InputManager.Instance.PlayerInput;
            _playerInput.Combat.Cycle.performed += CycleTarget;
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPlayerBeginAiming, EnterWeaponMode);
            //EventManager.Subscribe(EventTypes.OnPlayerConfirmShot, HandleShot);
            EventManager.Subscribe(EventTypes.OnPlayerEndAiming, ExitWeaponMode);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPlayerBeginAiming, EnterWeaponMode);
            //EventManager.Unsubscribe(EventTypes.OnPlayerConfirmShot, HandleShot);
            EventManager.Unsubscribe(EventTypes.OnPlayerEndAiming, ExitWeaponMode);
        }

        private void EnterWeaponMode()
        {
            if (!TurnManager.Instance.IsAllyTurn() || TurnManager.Instance.HasUnitActed(ActiveAllyController.ActiveAlly)) return; //_modeSwitcher == null

            //_modeSwitcher.SwitchMode(ActionType.Weapon);
            _validTargets = FindValidTargets();

            SetCurrentTarget();
        }

        private void ExitWeaponMode()
        {
            //if (!_modeSwitcher) return;

            //_modeSwitcher.SwitchMode(ActionType.Move); 
            reticle.Hide();
        }

        private void CycleTarget(InputAction.CallbackContext context) // SFX needed??
        {
            if (_validTargets.Count == 0 || !_aiming) return; //!_modeSwitcher

            float inputValue = context.ReadValue<float>();
            int direction = (inputValue > 0) ? -1 : 1;  

            _currentTargetIndex = (_currentTargetIndex + direction + _validTargets.Count) % _validTargets.Count;

            SetCurrentTarget();
        }

        private void HighlightTarget(Enemy target)
        {
            Transform centerOfMass = target.transform.Find("CenterOfMass");
            Vector3 targetPosition = centerOfMass != null ? centerOfMass.position : target.transform.position;

            reticle.SetPosition(Camera.main.WorldToScreenPoint(targetPosition));
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
            return true;
        }

        private void SetCurrentTarget()
        {
            if (_validTargets.Count > 0)
            {
                HighlightTarget(_validTargets[_currentTargetIndex]);
            }
            else
            {
                reticle.Hide();
            }
        }
    }

}
