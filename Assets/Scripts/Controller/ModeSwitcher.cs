using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Entities;
using Managers;
using UnityEngine;

namespace Controller 
{
    public enum ControlMode
    {
        StandardMove,
        AirSupportMove,
        Selection,
    }
    
    public class ModeSwitcher : PlayerComponent
    {
        public ControlMode CurrentMode;

        private void Awake()
        {
            SwitchMode(ControlMode.StandardMove);
        }

        public void SwitchMode(ControlMode newMode)
        {
            if (newMode == CurrentMode) return;

            CurrentMode = newMode;
            EventManager.TriggerEvent(EventTypes.OnPlayerChangeMode, CurrentMode);
        }
    }
}