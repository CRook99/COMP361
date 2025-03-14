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
        OnStartEnemyTurn, // on start enemy turn sequence
        OnEndEnemyTurn,
        OnEnemyBeginMove, // on start of enemy move sequence 
        OnEnemyEndMove,
        OnEnemyBeginShoot, // on start of enemy shooting sequence
        OnEnemyEndShoot, // on end of enemy shooting sequence
        OnSpawnAlly,
        OnSpawnEnemy,
        OnPause,
        OnUnpause,
        OnPlayerBeginAiming,
        OnPlayerConfirmShot,
        OnPlayerEndAiming,
        OnPlayerBeginAbility,
        OnPlayerEndAbility,
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

    public delegate void EventCallback(object data);
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

        private Dictionary<EventTypes, EventCallback> _events = new();
        
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
            SubscribeCallback(eventType, _ => listener());
        }
        
        public static void Subscribe(EventTypes eventType, Action<object> listener)
        {
            SubscribeCallback(eventType, listener.Invoke);
        }

        private static void SubscribeCallback(EventTypes eventType, EventCallback listener)
        {
            if (Instance._events.TryGetValue(eventType, out EventCallback thisEvent))
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
            UnsubscribeCallback(eventType, _ => listener());
        }

        public static void Unsubscribe(EventTypes eventType, Action<object> listener)
        {
            UnsubscribeCallback(eventType, listener.Invoke);
        }

        private static void UnsubscribeCallback(EventTypes eventType, EventCallback listener)
        {
            if (Instance._events.TryGetValue(eventType, out EventCallback thisEvent))
            {
                thisEvent -= listener;
                if (thisEvent == null) 
                    Instance._events.Remove(eventType);
                else 
                    Instance._events[eventType] = thisEvent;
            }
        }
        
        public static void TriggerEvent(EventTypes eventType, object data = null)
        {
            if (Instance._events.TryGetValue(eventType, out EventCallback thisEvent))
            {
                thisEvent?.Invoke(data);
            }
        }
    }
}