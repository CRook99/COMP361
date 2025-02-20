using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public enum EventTypes
    {
        OnCameraModeChanged,
        OnActiveAllyChanged,
        OnPlayerBeginMove,
        OnPlayerEndMove,
        OnPlayerUseAction,
        OnPlayerChangeMode,
        OnStartAllyTurn,
        OnStartEnemyTurn,
        OnSpawnAlly,
        OnSpawnEnemy,
        OnPause,
        OnUnpause,
        // Begin stats
        OnEnemyKilled,
        OnDamageDealt,
        OnShotLanded,
        OnSpaceMoved,
        OnAllyFallen,
        OnDamageTaken,
        OnShotTaken,
        OnChanceShotDodged
        // End stats
    }
    public class EventManager : MonoBehaviour
    {
        private static EventManager _instance;

        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("EventManager");
                    _instance = go.AddComponent<EventManager>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        private Dictionary<EventTypes, Action> _events = new();
        private Dictionary<EventTypes, Action<object>> _dataEvents = new();
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public static void Subscribe(EventTypes eventType, Action listener)
        {
            if (Instance._events.TryGetValue(eventType, out Action thisEvent))
            {
                Instance._events[eventType] = thisEvent + listener;
            }
            else
            {
                Instance._events[eventType] = listener;
            }
        }

        public static void Unsubscribe(EventTypes eventType, Action listener)
        {
            if (Instance._events.TryGetValue(eventType, out Action thisEvent))
            {
                Instance._events[eventType] = thisEvent - listener;
            }
        }

        public static void TriggerEvent(EventTypes eventType)
        {
            if (Instance._events.TryGetValue(eventType, out Action thisEvent))
            {
                thisEvent?.Invoke();
            }
        }

        // Methods for events with data
        public static void Subscribe(EventTypes eventType, Action<object> listener)
        {
            if (Instance._dataEvents.TryGetValue(eventType, out Action<object> thisEvent))
            {
                Instance._dataEvents[eventType] = thisEvent + listener;
            }
            else
            {
                Instance._dataEvents[eventType] = listener;
            }
        }

        public static void Unsubscribe(EventTypes eventType, Action<object> listener)
        {
            if (Instance._dataEvents.TryGetValue(eventType, out Action<object> thisEvent))
            {
                Instance._dataEvents[eventType] = thisEvent - listener;
            }
        }

        public static void TriggerEvent(EventTypes eventType, object data)
        {
            if (Instance._dataEvents.TryGetValue(eventType, out Action<object> thisEvent))
            {
                thisEvent?.Invoke(data);
            }
        }
    }
}