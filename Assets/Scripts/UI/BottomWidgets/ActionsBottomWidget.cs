using System;
using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;

namespace UI.BottomWidgets
{
    public abstract class ActionsBottomWidget : BottomWidget
    {
        [SerializeField] private PrimaryActionWidget actionWidgetPrefab;
        [SerializeField] private List<ActionScriptableObject> actions; // Centralize?
        
        protected Dictionary<ActionType, PrimaryActionWidget> _actionMap;

        protected override void Awake()
        {
            base.Awake();
            
            _actionMap = new();

            foreach (ActionScriptableObject action in actions)
            {
                PrimaryActionWidget widget = Instantiate(actionWidgetPrefab, transform);
                widget.Data = action;
                _actionMap.Add(action.Type, widget);
            }
        }

        protected virtual void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPlayerUseAction, OnUseAction);
        }
        
        protected virtual void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPlayerUseAction, OnUseAction);
            
        }

        private void OnUseAction(object data)
        {
            if (data is not ActionType type) return;

            if (_actionMap.TryGetValue(type, out PrimaryActionWidget widget))
            {
                _actionMap[type].Deactivate();
            }
        }

        // inherited by movement and air support base so always true
        public override bool CanOpen()
        {
            return true;
        }

        protected abstract void RefreshWidgets(object data);
    }
}