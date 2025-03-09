using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Entities;
using Managers;
using UnityEngine;

public class ModeSwitcher : PlayerComponent
{
    public ActionType CurrentMode;

    private void Awake()
    {
        SwitchMode(ActionType.Move);
    }

    public void SwitchMode(ActionType newMode)
    {
        if (newMode == CurrentMode) return;

        CurrentMode = newMode;
        EventManager.TriggerEvent(EventTypes.OnPlayerChangeMode, CurrentMode);
    }
}
