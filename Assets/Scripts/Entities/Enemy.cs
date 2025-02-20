using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework; // for Assert
using Unity.VisualScripting;
using UnityEngine;
using World;

namespace Entities
{
    public class Enemy : Entity
    {

        // Set of fields that determine how much Cover, Line of Sight enemies (given your in cover), and flanked by other allies
        // matters in the enemies decision making 
        private readonly int CoverWeight = 10;
        private readonly int LineOfSightInCoverWeight = 8;
        private readonly int isFlankedWeight = -10;

        // Get best cell the enemy should move based on current position
        public Cell GetBestMove()
        {
            List<Cell> obstacleCells = TacticsGrid.Instance.GetObstacleCells().ToList();
            List<Cell> playerPositions = GameManager.Allies.Select(a => a.CurrentCell).ToList();
            List<Cell> possibleMoves = GetAvailableCells(obstacleCells, playerPositions);

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

        // Gets available Cells based on current position, does not include obstacle cells or cells with enemies 
        private List<Cell> GetAvailableCells(List<Cell> obstacleCells, List<Cell> playerPositions)
        {
            Assert.NotNull(obstacleCells);
            Assert.NotNull(playerPositions);

            int currentX = CurrentCell.Position.x;
            int currentY = CurrentCell.Position.y;

            List<Cell> available = new List<Cell>();

            foreach (Cell cell in TacticsGrid.Instance.GetAllCells())
            {

                if (obstacleCells.Contains(cell) || playerPositions.Contains(cell) || !cell.Walkable)
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

        // Generates score for each cell depending on sime heuristics 
        private float EvalCell(Cell cell, List<Cell> playerPositions)
        {
            Assert.NotNull(cell);
            Assert.NotNull(playerPositions);

            float score = 0;

            // Prefer cover
            if (TacticsGrid.Instance.isCover(cell))
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
            Assert.NotNull(cell);
            Assert.NotNull(playerPositions);
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
            Assert.NotNull(cell);
            Assert.NotNull(playerPositions);

            HashSet<Cell> player_cells_in_sight = new HashSet<Cell>();

            foreach (Cell player_pos in playerPositions)
            {
                if (!TacticsGrid.Instance.ObstacleBetweenCells(cell, player_pos))
                {
                    player_cells_in_sight.Add(player_pos);
                }

                // if cell contains cover, then you can move out of cover to shoot
                if (TacticsGrid.Instance.isCover(cell))
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


        // for testing
        [ContextMenu("testMove")]
        private void testMove()
        {
            Cell best = GetBestMove();
            MoveToCell(best);
        }


        public override void TryMoveToCell(Cell destination)
        {
            throw new System.NotImplementedException();
        }
    }
}
