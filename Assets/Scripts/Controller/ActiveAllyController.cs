using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Controller
{
    public class ActiveAllyController : PlayerComponent
    {
        private Ally _activeAlly;
        private bool _isAiming;
        private List<Enemy> _validTargets = new List<Enemy>();

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
            while (GameManager.Allies.Count == 0)
            {
                yield return null;
            }
            
            ActiveAlly = GameManager.Allies[0];
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPlayerBeginAiming, StartAiming);
            EventManager.Subscribe(EventTypes.OnPlayerConfirmShot, HandleShot);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPlayerBeginAiming, StartAiming);
            EventManager.Unsubscribe(EventTypes.OnPlayerConfirmShot, HandleShot);
        }

        private void StartAiming()
        {
            // Prevent aiming when impossible
            if (_isAiming || TurnManager.Instance.HasUnitActed(ActiveAlly)) return;
            if (!TurnManager.Instance.IsAllyTurn()) return;

            _isAiming = true;
            _validTargets = FindValidTargets();
            

            // Lock unit switching while aiming
            EventManager.TriggerEvent(EventTypes.OnPlayerBeginAiming); 
        }

        private void HandleShot()
        {

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

        private bool IsTargetValid(Enemy enemy)
        {
            return false;
        }

    }
}