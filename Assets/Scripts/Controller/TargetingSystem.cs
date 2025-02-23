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
        private int _currentTargetIndex = 0; // Good practice to initialise here??
        private PlayerInput _playerInput;
        [SerializeField] private Reticle reticle;

        private void Update()
        {
            if (_validTargets.Count > 0)
            {
                reticle.SetPosition(_validTargets[_currentTargetIndex].transform.position);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                EventManager.TriggerEvent(EventTypes.OnPlayerBeginAiming);
            }
        }

        void Start()
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
            Debug.Log("Helo");
            if (!TurnManager.Instance.IsAllyTurn() || TurnManager.Instance.HasUnitActed(ActiveAllyController.ActiveAlly)) return; //_modeSwitcher == null
            Debug.Log("Helobai");

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
            if (_validTargets.Count == 0) return; //!_modeSwitcher

            float inputValue = context.ReadValue<float>();
            int direction = (inputValue > 0) ? -1 : 1;  

            _currentTargetIndex = (_currentTargetIndex + direction + _validTargets.Count) % _validTargets.Count;

            SetCurrentTarget();
        }

        private void HighlightTarget(Enemy target)
        {
            reticle.SetPosition(target.transform.position);
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
