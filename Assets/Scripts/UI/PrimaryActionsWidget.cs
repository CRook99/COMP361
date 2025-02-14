using System;
using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;

namespace UI
{
    public class PrimaryActionsWidget : MonoBehaviour
    {
        [SerializeField] private PrimaryActionWidget actionWidgetPrefab;
        [SerializeField] private List<ActionScriptableObject> actions; // Centralize?
        
        private Dictionary<ActionType, PrimaryActionWidget> _actionMap;

        private void Awake()
        {
            _actionMap = new();

            foreach (ActionScriptableObject action in actions)
            {
                PrimaryActionWidget widget = Instantiate(actionWidgetPrefab, transform);
                widget.Data = action;
                _actionMap.Add(action.Type, widget);
            }
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPlayerUseAction, OnUseAction);
            EventManager.Subscribe(EventTypes.OnActiveAllyChanged, RefreshWidgets);
        }
        
        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPlayerUseAction, OnUseAction);
        }

        private void OnUseAction(object data)
        {
            if (data is not ActionType type) return;
            
            _actionMap[type].Deactivate();
        }

        private void RefreshWidgets(object data)
        {
            if (data is not Ally activeAlly) return;

            foreach (var item in _actionMap)
            {
                if (activeAlly.Actions.CanUseAction(item.Key) && !item.Value.Active)
                {
                    item.Value.Activate();
                }
                else if (!activeAlly.Actions.CanUseAction(item.Key) && item.Value.Active)
                {
                    item.Value.Deactivate();
                }
            }
        }
    }
}