using System.Collections;
using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;

public class ModeSwitcher : MonoBehaviour
{
    private ActionType _currentMode;

    public void SwitchMode(ActionType newMode)
    {
        if (newMode == _currentMode) return;

        _currentMode = newMode;
        EventManager.TriggerEvent(EventTypes.OnPlayerChangeMode, _currentMode);
    }
}
