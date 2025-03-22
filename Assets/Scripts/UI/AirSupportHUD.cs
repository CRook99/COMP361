using System;
using System.Runtime.Remoting;
using Managers;
using UnityEngine;
using World;

public class AirSupportHUD : MonoBehaviour
{
    [SerializeField] private GameObject reticle;
    private Camera _camera;
    private LayerMask _cellLayer;

    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
        _camera = Camera.main;
        _cellLayer = LayerMask.GetMask("Cell");
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
