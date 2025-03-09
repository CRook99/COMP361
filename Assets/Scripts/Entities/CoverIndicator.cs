using System.Collections.Generic;
using UnityEngine;
using System;
using Entities;
using Managers;
using Controller;

[Serializable]
public struct CoverShield
{
    public GameObject Obj;
    public MeshFilter FillMesh;
}

public class CoverIndicator : PlayerComponent
{
    [SerializeField] private List<CoverShield> _shields;
    [Space]
    public Mesh HalfCoverMesh;
    public Mesh FullCoverMesh;
    
    public float StandardScale;
    public float StandardHeight = 0f;
    public float MeshDistance;
    public float AirSupportHeight;
    public float AirSupportScale;
    
    private Dictionary<CoverTypes, Mesh> _meshMap;
    private Quaternion UpwardsRotation = Quaternion.Euler(90f, 0f, 0f);
    private bool _shouldShowShields;

    private void Awake()
    {
        _shouldShowShields = true;
        
        _meshMap = new Dictionary<CoverTypes, Mesh>()
        {
            { CoverTypes.HalfCover, HalfCoverMesh },
            { CoverTypes.FullCover, FullCoverMesh }
        };
        
        var scale = Vector3.one * StandardScale;
        
        for (int i = 0; i < 4; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, 90f * i, 0f);
            Vector3 offset = rotation * Vector3.forward * MeshDistance;
            
            _shields[i].Obj.transform.SetLocalPositionAndRotation(offset, rotation);
            _shields[i].Obj.transform.localScale = scale;
        }
    }

    private void Update()
    {
        if (CameraController.Mode == CameraMode.AirSupport)
        {
            Debug.Log(CameraController.YRotation);
            
            for (int i = 0; i < 4; i++)
            {
                _shields[i].Obj.transform.localRotation = Quaternion.Euler(90f, CameraController.YRotation, 0f);
            }
        }
    }

    private void OnEnable()
    {
        MovementSelection.OnHoveredCellChanged += RefreshMeshes;
        EventManager.Subscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
        EventManager.Subscribe(EventTypes.OnPlayerBeginAiming, DisallowShields);
        EventManager.Subscribe(EventTypes.OnPlayerBeginMove, DisallowShields);
        EventManager.Subscribe(EventTypes.OnPlayerEndAiming, AllowShields);
        EventManager.Subscribe(EventTypes.OnPlayerEndMove, AllowShields);
        EventManager.Subscribe(EventTypes.OnStartEnemyTurn, DisallowShields);
        EventManager.Subscribe(EventTypes.OnEndEnemyTurn, AllowShields);

        // EventManager.Subscribe(EventTypes.OnPlayerBeginAbility, DisallowShields); TODO
        // EventManager.Subscribe(EventTypes.OnPlayerEndAbility, AllowShields); TODO
    }
    
    private void OnDisable()
    {
        MovementSelection.OnHoveredCellChanged -= RefreshMeshes;
        EventManager.Unsubscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
        EventManager.Unsubscribe(EventTypes.OnPlayerBeginAiming, DisallowShields);
        EventManager.Unsubscribe(EventTypes.OnPlayerBeginMove, DisallowShields);
        EventManager.Unsubscribe(EventTypes.OnPlayerEndAiming, AllowShields);
        EventManager.Unsubscribe(EventTypes.OnPlayerEndMove, AllowShields);
        EventManager.Unsubscribe(EventTypes.OnStartEnemyTurn, DisallowShields);
        EventManager.Unsubscribe(EventTypes.OnEndEnemyTurn, AllowShields);
        // EventManager.Unsubscribe(EventTypes.OnPlayerBeginAbility, DisallowShields); TODO
        // EventManager.Unsubscribe(EventTypes.OnPlayerEndAbility, AllowShields); TODO
    }

    private void RefreshMeshes(Cell cell)
    {
        if (!_shouldShowShields) return;
        
        for (int i = 0; i < 4; i++)
        {
            CoverShield shield = _shields[i];
            Cell neighbour = cell.Neighbours[i * 2];
            
            if (neighbour == null || neighbour.Cover == CoverTypes.NoCover || !cell.Walkable)
            {
                shield.Obj.SetActive(false);
            }
            else
            {
                shield.Obj.SetActive(true);
                shield.FillMesh.mesh = _meshMap[neighbour.Cover];
            }

            _shields[i] = shield;
        }
    }

    private void AllowShields()
    {
        _shouldShowShields = true;
    }

    private void DisallowShields()
    {
        _shouldShowShields = false;
        
        foreach (CoverShield shield in _shields)
        {
            shield.Obj.SetActive(false);
        }
    }
    
    // Event subscriptions
    private void OnCameraModeChanged(object data)
    {
        if (data is not CameraMode mode) return;

        for (int i = 0; i < 4; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, 90f * i, 0f);
            Vector3 offset = rotation * Vector3.forward * MeshDistance;

            if (mode == CameraMode.AirSupport)
            {
                offset += Vector3.up * AirSupportHeight;
                rotation = Quaternion.Euler(90f, 0f, 0f);
            }
            
            _shields[i].Obj.transform.SetLocalPositionAndRotation(offset, rotation);
            _shields[i].Obj.transform.localScale = Vector3.one * (mode == CameraMode.Standard
                ? StandardScale
                : AirSupportScale);
        }
    }
}