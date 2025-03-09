using System.Collections.Generic;
using UnityEngine;
using System;
using Entities;
using Managers;
using World;
using System.Runtime.ConstrainedExecution;
using Controller;
using Utility;

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
    
    public float ScaleFactor;
    public float MeshDistance;
    
    private Dictionary<CoverTypes, Mesh> _meshMap;

    private void Awake()
    {
        _meshMap = new Dictionary<CoverTypes, Mesh>()
        {
            { CoverTypes.HalfCover, HalfCoverMesh },
            { CoverTypes.FullCover, FullCoverMesh }
        };
        
        var scale = Vector3.one * ScaleFactor;
        
        for (int i = 0; i < 4; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, 90f * i, 0f);
            Vector3 offset = rotation * Vector3.forward * MeshDistance;
            
            _shields[i].Obj.transform.SetLocalPositionAndRotation(offset, rotation);
            _shields[i].Obj.transform.localScale = scale;
        }
    }

    private void OnEnable()
    {
        MovementSelection.OnHoveredCellChanged += RefreshMeshes;
    }
    
    private void OnDisable()
    {
        MovementSelection.OnHoveredCellChanged -= RefreshMeshes;
    }

    private void RefreshMeshes(Cell cell)
    {
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
}