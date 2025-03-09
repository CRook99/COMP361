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
    public class ActiveAllyController : PlayerComponent // Locking? Event Subscription
    {
        private Ally _activeAlly;

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
            EventManager.Subscribe(EventTypes.OnPlayerChangeMode, OnPlayerChangeMode);
        }
        
        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPlayerChangeMode, OnPlayerChangeMode);
        }

        private void OnPlayerChangeMode(object data)
        {
            if (data is not ControlMode mode)
            {
                Debug.LogWarning("Passed incorrect type to ActiveAllyController::OnPlayerChangeMode");
                return;
            }

            _activeAlly.SetMoveMeshActive(mode == ControlMode.StandardMove && _activeAlly.Actions.CanUseAction(ActionType.Move));
        }
    }
}