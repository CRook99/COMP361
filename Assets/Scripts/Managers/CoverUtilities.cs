using System.Collections.Generic;
using UnityEngine;
using System;
using Entities;
using Managers;
using Utility;
using World;

public static class CoverUtilities
{
    public enum OrthDir
    {
        N,
        E,
        S,
        W
    }

    public static CoverTypes GetCellCover(Cell cell, OrthDir direction)
    {
        switch (direction)
        {
            case OrthDir.N:
                return cell.N.Cover;
            case OrthDir.E:
                return cell.E.Cover;
            case OrthDir.S:
                return cell.S.Cover;
            case OrthDir.W:
                return cell.W.Cover;
            default:
                return CoverTypes.NoCover;
        }
    }

    /**
     * Given cells origin and destination, determine what kind of cover destination has from origin.
     * That is, the cover object that destination is directly next to, from the angle-majority direction
     */
    public static CoverTypes GetImmediateCoverLevel(Cell origin, Cell target, out GameObject coverObject)
    {
        // Angle of ray from origin to target relative to x axis
        float angle = Mathf.Atan2(target.Position.y - origin.Position.y, target.Position.x - origin.Position.x) * Mathf.Rad2Deg;
        OrthDir[] directions = { OrthDir.E, OrthDir.N, OrthDir.W, OrthDir.S};
        // Direction shot is coming from relative to target
        OrthDir shotOriginDirection = directions[Mathf.RoundToInt((angle + 180f) / 90f) % 4];
        Cell neighbour;
        switch (shotOriginDirection)
        {
            case OrthDir.W:
                neighbour = target.W;
                break;
                //return target.W?.Cover ?? CoverTypes.NoCover;
            case OrthDir.S:
                neighbour = target.S;
                break;
            case OrthDir.E:
                neighbour = target.E;
                break;
            case OrthDir.N:
                neighbour = target.N;
                break;
            default:
                neighbour = target.N;
                break;
        }
        
        RaycastHit hit;
        if (Physics.Raycast(neighbour.Position.ToVector3XZ(0f), Vector3.up, out hit, 1f))
        {
            coverObject = hit.transform.gameObject;
        }
        else
        {
            coverObject = null;
        }

        return neighbour?.Cover ?? CoverTypes.NoCover;
    }
}