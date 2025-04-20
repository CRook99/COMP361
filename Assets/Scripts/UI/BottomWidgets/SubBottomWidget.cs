using System;
using Controller;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
    public class SubBottomWidget : BottomWidget
    {
        [SerializeField] protected ActionType ActionType;

        public override void Open()
        {
            base.Open();
            
            EventManager.TriggerEvent(EventTypes.OnPlayerChangeMode, ControlMode.Selection);
        }
        
        public override bool CanOpen()
        {
            return _playerReferences.ActiveAllyController.ActiveAlly.Actions.CanUseAction(ActionType);
        }
    }
}