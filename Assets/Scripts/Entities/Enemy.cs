using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World;
using Managers;
using System.Collections;


namespace Entities
{
  public class Enemy : Entity
  {
    // Set of fields that determine how much Cover, Line of Sight enemies (given your in cover), and flanked by other allies
    // matters in the enemies decision making 
    private readonly int CoverWeight = 10;
    private readonly int LineOfSightInCoverWeight = 8;
    private readonly int isFlankedWeight = -10;
    private int Disarmed = 0;

    // Get best cell the enemy should move based on current position
    public Cell GetBestMove()
    {
      List<Cell> obstacleCells = TacticsGrid.Instance.GetCoverCells().ToList();
      List<Cell> playerPositions = GameManager.Allies.Select(a => a.CurrentCell).ToList();
      List<Cell> EnemyPositions = GameManager.Enemies.Select(a => a.CurrentCell).ToList();
      List<Cell> possibleMoves = GetAvailableCells(obstacleCells, playerPositions, EnemyPositions);

      Cell bestMove = null;
      float bestScore = float.MinValue;

      foreach (Cell cell in possibleMoves)
      {
        float score = EvalCell(cell, playerPositions);
        if (score > bestScore)
        {
          bestScore = score;
          bestMove = cell;
        }
      }

      return bestMove;
    }

    protected override void Start()
    {
      base.Start();
      EventManager.TriggerEvent(EventTypes.OnSpawnEnemyStartGame, this);
      EventManager.TriggerEvent(EventTypes.OnSpawnEnemyLoadHealthBar, this);
    }

    // Gets available Cells based on current position, does not include obstacle cells or cells with enemies 
    private List<Cell> GetAvailableCells(List<Cell> obstacleCells, List<Cell> playerPositions, List<Cell> EnemyPositions)
    {
      if (obstacleCells == null)
        throw new ArgumentNullException("GetAvailableCells called with null obstacleCells");
      if (playerPositions == null)
        throw new ArgumentNullException("GetAvailableCells called with null playerPositions");

      int currentX = CurrentCell.Position.x;
      int currentY = CurrentCell.Position.y;

      List<Cell> available = new List<Cell>();

      foreach (Cell cell in TacticsGrid.Instance.GetAllCells())
      {

        if (obstacleCells.Contains(cell) || playerPositions.Contains(cell) || EnemyPositions.Contains(cell) || !cell.Walkable)
        {
          continue;
        }

        // Calculate distance
        int cellX = cell.Position.x;
        int cellY = cell.Position.y;
        float distance = Mathf.Sqrt(Mathf.Pow(cellX - currentX, 2) + Mathf.Pow(cellY - currentY, 2));

        if (distance <= Data.MovementRange)
        {
          available.Add(cell);
        }

      }

      return available;
    }

    // Generates score for each cell depending on some heuristics 
    private float EvalCell(Cell cell, List<Cell> playerPositions)
    {
      if (cell == null)
        throw new ArgumentNullException("EvalCell called with null cell");
      if (playerPositions == null)
        throw new ArgumentNullException("EvalCell called with null playerPositions");

      float score = 0;

      // Prefer cover
      if (TacticsGrid.Instance.IsCover(cell))
      {
        score += CoverWeight;

        // Ensure attack chance
        score += LineOfSightInCoverWeight * HasLineOfSight(cell, playerPositions);
        score += isFlankedWeight * GetFlankedCount(cell, playerPositions);
      }
      else
      {
        // Avoid flanking
        score += isFlankedWeight * GetFlankedCount(cell, playerPositions);
      }

      return score;
    }

    // Gets number of allies that would flank the enemy at cell
    private int GetFlankedCount(Cell cell, List<Cell> playerPositions)
    {
      if (cell == null)
        throw new ArgumentNullException("GetFlankedCount called with null cell");
      if (playerPositions == null)
        throw new ArgumentNullException("GetFlankedCount called with null playerPositions");

      int flanked_by = 0;
      foreach (Cell player_pos in playerPositions)
      {
        if (!TacticsGrid.Instance.ObstacleBetweenCells(player_pos, cell))
        {
          flanked_by += 1;
        }
      }
      return flanked_by;
    }

    // Gets number of players that are in enemy line of sight
    // if cell is a cover, then its allows to move to the side to shoot
    private int HasLineOfSight(Cell cell, List<Cell> playerPositions)
    {
      if (cell == null)
        throw new ArgumentNullException("HasLineOfSight called with null cell");
      if (playerPositions == null)
        throw new ArgumentNullException("HasLineOfSight called with null playerPositions");

      HashSet<Cell> player_cells_in_sight = new HashSet<Cell>();

      foreach (Cell player_pos in playerPositions)
      {
        if (!TacticsGrid.Instance.ObstacleBetweenCells(cell, player_pos))
        {
          player_cells_in_sight.Add(player_pos);
        }

        // if cell contains cover, then you can move out of cover to shoot
        if (TacticsGrid.Instance.IsCover(cell))
        {
          if (cell.N != null && cell.N.Walkable &&
          !TacticsGrid.Instance.ObstacleBetweenCells(cell.N, player_pos))
            player_cells_in_sight.Add(player_pos);

          if (cell.S != null && cell.S.Walkable &&
          !TacticsGrid.Instance.ObstacleBetweenCells(cell.S, player_pos))
            player_cells_in_sight.Add(player_pos);

          if (cell.E != null && cell.E.Walkable &&
          !TacticsGrid.Instance.ObstacleBetweenCells(cell.E, player_pos))
            player_cells_in_sight.Add(player_pos);

          if (cell.W != null && cell.W.Walkable &&
          !TacticsGrid.Instance.ObstacleBetweenCells(cell.W, player_pos))
            player_cells_in_sight.Add(player_pos);
        }
      }

      return player_cells_in_sight.Count;
    }

    // Finds the closest visible potential ally to shoot, and the cell to shoot from
    public (Cell, Ally) FindClosestVisibleAllyToShoot()
    {
      Ally closestAlly = null;
      float closestDistance = float.MaxValue;
      Cell shootingCell = null; // Cell to shoot from

      foreach (Ally ally in GameManager.Allies)
      {
        (Cell cell, float distance) = FindPeakableCellInLightOfSight(ally);

        if (distance < closestDistance)
        {
            closestDistance = distance;
            closestAlly = ally;
            shootingCell = cell;
        }
      }
      return (shootingCell, closestAlly);
    }

    // Helper function to check if there is an ally in (peakable) view of the enemy
    public bool ThereIsAllyToShoot() {
        List<Cell> playerPositions = GameManager.Allies.Select(a => a.CurrentCell).ToList();
        foreach (Ally ally in GameManager.Allies)
        {
            (Cell cell, float distance) = FindPeakableCellInLightOfSight(ally);

            if(cell != null)
                return true;
        }
        return false;
        
    }

    // Moves the enemy to the cell during its enemy turn
    public override void TryMoveToCell(Cell destination)
    {
      StartCoroutine(MoveToCell(destination));
    }

    protected override IEnumerator FollowPath(List<Cell> path)
    {
      EventManager.TriggerEvent(EventTypes.OnEnemyBeginMove, this);
      yield return base.FollowPath(path);
      EventManager.TriggerEvent(EventTypes.OnEnemyEndMove);
    }

    public void Disarm(int turns)
    {
      Disarmed = turns;
    }

    public bool IsDisarmed()
    {
      if (Disarmed == 0) return false;
      Disarmed--;
      Debug.Log("Enemy has " + Disarmed + " turns to go");
      return true;
    }

    [ContextMenu("Kill Enemy Test")]
    protected override void Die() {
        Debug.Log($"{gameObject.name} died!");
        
        EventManager.TriggerEvent(EventTypes.OnEnemyKilled, this);
        
        // Destroys Enemy Object
        base.Die();
    }

    public override void TakeDamage(int amount)
    {
      base.TakeDamage(amount);
      
      EventManager.TriggerEvent(EventTypes.OnDamageDealt, amount); // stats manager
    }
  }
}