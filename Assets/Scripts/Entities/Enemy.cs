using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using World;

namespace Entities
{
    public class Enemy : Entity
    { 
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

            foreach (Cell cell in TacticsGrid.Instance.GetAllCells()) {

                if(obstacleCells.Contains(cell) || playerPositions.Contains(cell) || !cell.Walkable) {
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
            if (IsCover(cell)) score += 10f;

            // Avoid flanking
            score -= 10f * IsFlanked(cell, playerPositions);

            // Ensure attack chance
            score += 5f * HasLineOfSight(cell, playerPositions);
            print(playerPositions);

            return score;
        }

        // checks if any adjacent cell is an obstacle (cover)
        private bool IsCover(Cell cell)
        {
            Assert.NotNull(cell);
            HashSet<Cell> obstacles = TacticsGrid.Instance.GetObstacleCells();   
            return 
            obstacles.Contains(cell.N) ||
            obstacles.Contains(cell.S) ||
            obstacles.Contains(cell.W) ||
            obstacles.Contains(cell.E);
        }

        // Gets number of allies that would flank the enemy at cell
        private int IsFlanked(Cell cell, List<Cell> playerPositions)
        {
            Assert.NotNull(cell);
            Assert.NotNull(playerPositions);

            int flanked_by = 0;
            HashSet<Cell> obstacles = TacticsGrid.Instance.GetObstacleCells();

            foreach(Cell player_pos in playerPositions) {
                if(!HasObstacleBetween(player_pos, cell, obstacles)) {
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

            HashSet<Cell> obstacles = TacticsGrid.Instance.GetObstacleCells();

            HashSet<Cell> player_cells_in_sight = new HashSet<Cell>();

            foreach(Cell player_pos in playerPositions) {
                if(!HasObstacleBetween(cell, player_pos, obstacles)) {
                    obstacles.Add(player_pos );
                }

                // if cell contains cover, then you can move out of cover to shoot
                if(IsCover(cell)) {
                    if(cell.N != null && cell.N.Walkable && !HasObstacleBetween(cell.N, player_pos, obstacles))
                        obstacles.Add(player_pos);

                    if(cell.S != null && cell.S.Walkable && !HasObstacleBetween(cell.S, player_pos, obstacles))
                        obstacles.Add(player_pos);

                    if(cell.E != null && cell.E.Walkable && !HasObstacleBetween(cell.E, player_pos, obstacles))
                        obstacles.Add(player_pos);

                    if(cell.W != null && cell.W.Walkable && !HasObstacleBetween(cell.W, player_pos, obstacles))
                        obstacles.Add(player_pos);
                    
                }
            }

            return player_cells_in_sight.Count;
        }

        // given start and end cells, checks if an obstacle exists somewhere between them
        private bool HasObstacleBetween(Cell start, Cell end, HashSet<Cell> obstacles)
        {
            int dx = (int)Mathf.Sign(end.Position.x - start.Position.x);
            int dy = (int)Mathf.Sign(end.Position.y - start.Position.y);

            int x = start.Position.x + dx;
            int y = start.Position.y + dy;

            while (x != end.Position.x || y != end.Position.y)
            {
                Cell checkCell = TacticsGrid.Instance.GetCell(x, y);
                if (obstacles.Contains(checkCell)) 
                    return true;

                x += dx;
                y += dy;
            }

            return false;
        }

        // for testing
        private void Update() {
            if (Input.GetKeyDown(KeyCode.M))
            {
                Cell best = GetBestMove();
                MoveToCell(best);
            }
        }
    }
}
