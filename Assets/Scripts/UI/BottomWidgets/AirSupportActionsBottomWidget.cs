using System;
using System.Collections.Generic;
using Controller;
using Entities;
using Managers;
using UnityEngine;

namespace UI.BottomWidgets
{
    public class AirSupportActionsBottomWidget : ActionsBottomWidget
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            
            EventManager.Subscribe(EventTypes.OnEndEnemyTurn, OnEndEnemyTurn);
            EventManager.Subscribe(EventTypes.OnCameraModeChanged, RefreshWidgets);
        }

        private void Start()
        {
            if (AirSupportManager.Instance != null)
            {
                AirSupportManager.Instance.Actions.OnUseAction += OnUseAction;
                AirSupportManager.Instance.Actions.OnCooldownChanged += OnCooldownChanged;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            EventManager.Unsubscribe(EventTypes.OnEndEnemyTurn, OnEndEnemyTurn);
            EventManager.Unsubscribe(EventTypes.OnCameraModeChanged, RefreshWidgets);
        }

        private void OnUseAction(ActionType type)
        {
            int cooldown = AirSupportManager.Instance.Actions.GetCooldown(type);
            OnCooldownChanged(type, cooldown);
        }

        private void OnEndEnemyTurn()
        {
            AirSupportManager.Instance.Actions.Tick();
            RefreshWidgets(null);
        }
        
        protected override void RefreshWidgets(object _)
        {
            foreach (var kvp in _actionMap)
            {
                ActionType type = kvp.Key;
                PrimaryActionWidget widget = kvp.Value;
                
                widget.UpdateCooldown(AirSupportManager.Instance.Actions.GetCooldown(type));
                
                if (AirSupportManager.Instance.Actions.CanUseAction(type))
                {
                    widget.Activate();
                }
                else
                {
                    widget.Deactivate();
                }
                
                widget.UpdateCooldown(AirSupportManager.Instance.Actions.GetCooldown(type));
            }
        }
    }
}