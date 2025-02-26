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
    }
}