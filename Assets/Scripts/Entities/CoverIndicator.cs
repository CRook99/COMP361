using System.Collections.Generic;
using UnityEngine;
using System;
using Entities;
using Managers;
using World;
using System.Runtime.ConstrainedExecution;

public class CoverIndicator : MonoBehaviour
 {
    private static CoverIndicator Instance;
    public Mesh NoCoverMesh;
    public Mesh HalfCoverMesh;
    public Mesh FullCoverMesh;
    public Mesh CoverCompromisedMesh;
    public Material Material;
    private Matrix4x4[] Matrices;
    private Quaternion[] Rotations;
    private Vector3 Scale;

    [Range(0.1f, 1.0f)]
    public readonly float ScaleFactor;

    [Range(0.1f, 0.6f)]
    public readonly float MeshDistance;
    private Mesh MeshInUse;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Rotations[0] = Quaternion.identity;
        Rotations[1] = Quaternion.Euler(0f, 90f, 0f);
        Rotations[2] = Quaternion.Euler(0f, 180f, 0f);
        Rotations[3] = Quaternion.Euler(0f, 270f, 0f);
        Scale = Vector3.one * ScaleFactor;
    }

    //Set up the display matrices for the new cover indicators
    public void ChangeCoverDisplay(Entity entity)
    {
        if (entity.CoverCompromised)
            MeshInUse = CoverCompromisedMesh;
        else switch (entity.Cover)
        {
            case CoverTypes.NoCover: 
                MeshInUse = NoCoverMesh;
                break;
            case CoverTypes.HalfCover:
                MeshInUse = HalfCoverMesh;
                break;
            case CoverTypes.FullCover:
                MeshInUse = FullCoverMesh;
                break;
        }

        Matrices = new Matrix4x4[4];
        Vector3[] positions = new Vector3[4];
        Vector3 p = entity.transform.position;

        positions[0] = p + new Vector3(0f, 0f, MeshDistance);
        positions[1] = p + new Vector3(MeshDistance, 0f, 0f);
        positions[2] = p + new Vector3(0f, 0f, -MeshDistance);
        positions[3] = p + new Vector3(-MeshDistance, 0f, 0f);

        Matrices[0] = Matrix4x4.TRS(positions[0], Rotations[0], Scale);
        Matrices[1] = Matrix4x4.TRS(positions[1], Rotations[1], Scale);
        Matrices[2] = Matrix4x4.TRS(positions[2], Rotations[2], Scale);
        Matrices[3] = Matrix4x4.TRS(positions[3], Rotations[3], Scale);
    }

    //Draw different meshes depending on the state of the entity's cover
    public void DisplayCoverStatus()
    {
        Graphics.DrawMeshInstanced(MeshInUse, 0, Material, Matrices);
    }
}