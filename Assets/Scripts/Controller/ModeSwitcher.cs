using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Entities;
using Managers;
using UnityEngine;

public class ModeSwitcher : PlayerComponent
{
    private ActionType _currentMode;

    private void Awake()
    {
        SwitchMode(ActionType.Move);
    }

    public void SwitchMode(ActionType newMode)
    {
        if (newMode == _currentMode) return;

        _currentMode = newMode;
        EventManager.TriggerEvent(EventTypes.OnPlayerChangeMode, _currentMode);
    }
}
