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
    
    // private static CoverIndicator Instance;
    
    // public Mesh CoverCompromisedMesh;
    // public Material Material;
    // private Matrix4x4[] Matrices;
    
    //
    // public GameObject shield;
    //
    
    // private Mesh MeshInUse;
    //
    // private Dictionary<CoverTypes, Mesh> _meshMap;
    //
    // private void Awake()
    // {
    //     if (Instance != null && Instance != this)
    //     {
    //         Destroy(this);
    //     }
    //     else
    //     {
    //         Instance = this;
    //     }
    //
    
    //
    //     Matrices = new Matrix4x4[4];
    //     Matrices[0] = Matrix4x4.TRS(Vector3.zero, Rotations[0], Scale);
    //     Matrices[1] = Matrix4x4.TRS(Vector3.zero, Rotations[1], Scale);
    //     Matrices[2] = Matrix4x4.TRS(Vector3.zero, Rotations[2], Scale);
    //     Matrices[3] = Matrix4x4.TRS(Vector3.zero, Rotations[3], Scale);
    //
    //     _meshMap = new Dictionary<CoverTypes, Mesh>()
    //     {
    //         { CoverTypes.NoCover, NoCoverMesh },
    //         { CoverTypes.HalfCover, HalfCoverMesh },
    //         { CoverTypes.FullCover, FullCoverMesh }
    //     };
    //
    //     MeshInUse = FullCoverMesh;
    // }
    //
    // private void Update()
    // {
    //     Graphics.DrawMeshInstanced(MeshInUse, 0, Material, Matrices);
    // }
    //
    // private void OnEnable()
    // {
    //     MovementSelection.OnHoveredCellChanged += ChangeCoverDisplay;
    // }
    //
    // private void OnDisable()
    // {
    //     MovementSelection.OnHoveredCellChanged -= ChangeCoverDisplay;
    // }
    //
    // //Set up the display matrices for the new cover indicators
    // public void ChangeCoverDisplay(Cell cell)
    // {
    //     Debug.Log(cell);
    //     // if (entity.CoverCompromised)
    //     //     MeshInUse = CoverCompromisedMesh;
    //     // else switch (entity.Cover)
    //     // {
    //     //     case CoverTypes.NoCover: 
    //     //         MeshInUse = NoCoverMesh;
    //     //         break;
    //     //     case CoverTypes.HalfCover:
    //     //         MeshInUse = HalfCoverMesh;
    //     //         break;
    //     //     case CoverTypes.FullCover:
    //     //         MeshInUse = FullCoverMesh;
    //     //         break;
    //     // }
    //
    //     Matrices = new Matrix4x4[4];
    //     Vector3[] positions = new Vector3[4];
    //     Mesh[] meshes = new Mesh[4];
    //     Vector3 p = cell.Position.ToVector3XZ(1f);
    //     
    //     positions[0] = p + new Vector3(0f, 0f, MeshDistance);
    //     positions[1] = p + new Vector3(MeshDistance, 0f, 0f);
    //     positions[2] = p + new Vector3(0f, 0f, -MeshDistance);
    //     positions[3] = p + new Vector3(-MeshDistance, 0f, 0f);
    //
    //     // Matrices[0] = Matrix4x4.TRS(positions[0], Rotations[0], Scale);
    //     // Matrices[1] = Matrix4x4.TRS(positions[1], Rotations[1], Scale);
    //     // Matrices[2] = Matrix4x4.TRS(positions[2], Rotations[2], Scale);
    //     // Matrices[3] = Matrix4x4.TRS(positions[3], Rotations[3], Scale);
    //
    //     for (int i = 0; i < 4; i++)
    //     {
    //         Matrices[i] = Matrix4x4.TRS(positions[0], Rotations[0], Scale);
    //         meshes[i] = _meshMap[cell.Neighbours[i * 2].Cover];
    //     }
    // }
}