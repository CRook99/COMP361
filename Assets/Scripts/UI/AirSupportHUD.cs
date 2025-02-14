using System;
using System.Runtime.Remoting;
using Managers;
using UnityEngine;

public class AirSupportHUD : MonoBehaviour
{
    [SerializeField] private GameObject reticle;

    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
    }

    private void OnCameraModeChanged(object data)
    {
        if (data is not CameraMode mode) return;

        if (mode == CameraMode.AirSupport)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    private void Activate()
    {
        reticle.SetActive(true);
    }

    private void Deactivate()
    {
        reticle.SetActive(false);
    }
    
    
}
