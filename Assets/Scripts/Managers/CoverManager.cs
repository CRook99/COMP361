using System.Collections.Generic;
using UnityEngine;
using System;
using Entities;
using Managers;
using World;

public class CoverManager : MonoBehaviour
{
    public enum OrthDir
    {
        N,
        E,
        S,
        W
    }
    
    public static CoverManager Instance;
    private static CoverIndicator CoverDisplay;
    //private Dictionary<Entity, CoverTypes> EntityCovers;
    // private Dictionary<Cell, GameObject> Covers;
    // private Entity Previous;

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
    }
    //
    //     Covers = TacticsGrid.Instance.GetCovers();
    //     Previous = null;
    //
    //     var entities = FindObjectsByType<Entity>(FindObjectsSortMode.None);
    //
    //     foreach (Entity e in entities)
    //     {
    //         EntityCovers.Add(e, CoverTypes.NoCover);
    //     }
    //
    //     EventManager.Subscribe(EventTypes.OnEndEnemyTurn, OnEndEnemyTurn);
    //     EventManager.Subscribe(EventTypes.OnPlayerEndMove, OnPlayerEndMove);
    //     EventManager.Subscribe(EventTypes.OnActiveAllyChanged, OnActiveAllyChanged);
    //     EventManager.Subscribe(EventTypes.OnActiveAllyChanged, OnPlayerUseAction);
    // }
    //
    // public void SetCover(Entity entity)
    // {
    //     Cell cell = entity.CurrentCell;
    //
    //     if (!TacticsGrid.Instance.IsCover(cell))
    //     {
    //         entity.Cover = CoverTypes.NoCover;
    //         return;
    //     }
    //
    //     CoverTypes cover = GetCellCover(cell);
    //     entity.Cover = cover;
    //     EntityCovers[entity] = cover;
    // }

    // public bool IsInCover(Entity entity)
    // {
    //     if (entity == null) return false;
    //
    //     CoverTypes cover = entity.Cover;
    //     return cover == CoverTypes.HalfCover || cover == CoverTypes.FullCover;
    // }

    // public void OnPlayerEndMove()
    // {
    //     foreach (var e in EntityCovers)
    //     {
    //         Entity ee = e.Key;
    //         if (ee is Ally) 
    //         {
    //             SetCover(ee);
    //             IsCoverCompromised(ee, true);
    //         }
    //     }
    //
    //     if (Previous != null)
    //         OnPlayerUseAction(Previous);
    //     Previous = null;
    // }
    //
    // public void OnEndEnemyTurn()
    // {
    //     foreach (var e in EntityCovers)
    //     {
    //         Entity ee = e.Key;
    //         if (ee is Enemy) 
    //         {
    //             SetCover(ee);
    //             IsCoverCompromised(ee, false);
    //         }
    //     }
    // }

    // public void OnActiveAllyChanged(object data)
    // {
    //     if (data is not Ally ally) 
    //     {
    //         Debug.Log("Tried to change to something that wasn't an ally");
    //         return;
    //     }
    //
    //     //Switch off the previous ally just to be sure
    //     if (Previous != null)
    //         OnPlayerUseAction(Previous);
    //     
    //     ally.CoverModeHighlighted = true;
    //     Previous = ally;
    //     CoverDisplay.ChangeCoverDisplay(ally);
    // }
    //
    // public void OnPlayerUseAction(object data)
    // {
    //     if (data is not Ally ally) 
    //     {
    //         Debug.Log("Tried to act with something that wasn't an ally");
    //         return;
    //     }
    //
    //     ally.CoverModeHighlighted = false;
    // }

    //If we're surrounded by multiple kinds of cover, check what the highest nearby cover level is
    // public CoverTypes GetCellCover(Cell cell)
    // {
    //     if (TacticsGrid.Instance.IsCover(cell))
    //     {
    //         CoverTypes cover = CoverTypes.HalfCover;
    //
    //         if (CoverType(Covers[cell.N]) == CoverTypes.FullCover
    //             || CoverType(Covers[cell.E]) == CoverTypes.FullCover
    //             || CoverType(Covers[cell.S]) == CoverTypes.FullCover
    //             || CoverType(Covers[cell.W]) == CoverTypes.FullCover)
    //             {
    //                 cover = CoverTypes.FullCover;
    //             }
    //         
    //         return cover;
    //     }
    //     else return CoverTypes.NoCover;
    // }

    public CoverTypes GetCellCover(Cell cell, OrthDir direction)
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

    //If allies == true, then check if friendly cover is compromised
    //else check enemy cover
    // public bool IsCoverCompromised(Entity entity, bool allies)
    // {
    //     var Type = allies ? typeof(Enemy) : typeof(Ally);
    //
    //     foreach (var e in EntityCovers)
    //     {
    //         Entity ee = e.Key;
    //         if (ee.GetType() == Type)
    //         {
    //             if (!ee.HasObstacleBetween(entity.CurrentCell))
    //                 return true;
    //         }
    //     }
    //
    //     //No lines of sight between the entity and an opposing force
    //     return false;
    // }

    /**
     * Given cells origin and destination, determine what kind of cover destination has from origin.
     * That is, the cover object that destination is directly next to, from the angle-majority direction
     */
    public CoverTypes GetImmediateCoverOfTargetFromOrigin(Cell origin, Cell target)
    {
        // Angle of ray from origin to target relative to x axis
        float angle = Mathf.Atan2(target.Position.y - origin.Position.y, target.Position.x - origin.Position.x) * Mathf.Rad2Deg;
        OrthDir[] directions = { OrthDir.E, OrthDir.N, OrthDir.W, OrthDir.S};
        // Direction shot is coming from relative to target
        OrthDir shotOriginDirection = directions[Mathf.RoundToInt((angle + 180f) / 90f) % 4];
        switch (shotOriginDirection)
        {
            case OrthDir.W:
                return target.W?.Cover ?? CoverTypes.NoCover;
            case OrthDir.S:
                return target.S?.Cover ?? CoverTypes.NoCover;
            case OrthDir.E:
                return target.E?.Cover ?? CoverTypes.NoCover;
            case OrthDir.N:
                return target.N?.Cover ?? CoverTypes.NoCover;
            default:
                return CoverTypes.NoCover;
        }
    }

    public static void DisplayCoverStatus()
    {
        CoverDisplay.DisplayCoverStatus();
    }

    public static CoverTypes CoverType(GameObject obj)
    {
        if (obj == null) return CoverTypes.NoCover;

        String name = obj.name;

        if (name.Contains("FullCover")) return CoverTypes.FullCover;
        else if (name.Contains("HalfCover")) return CoverTypes.HalfCover;
        else return CoverTypes.NoCover;
    }

}