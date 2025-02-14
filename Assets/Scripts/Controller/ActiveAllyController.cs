using System;
using System.Collections;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Controller
{
    public class ActiveAllyController : PlayerComponent
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