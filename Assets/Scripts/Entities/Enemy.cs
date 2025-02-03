using System.Collections.Generic;
using UnityEngine;
using World;

namespace Entities
{
    public class Enemy : Entity
    { 
        // Get best cell the enemy should move based on current position

        public Cell GetBestMove(List<Cell> coverCells, List<Cell> playerPositions)
        {   
            List<Cell> possibleMoves = GetAvailableCells();

            Cell bestMove = null;
            float bestScore = float.MinValue;

            foreach (Cell cell in possibleMoves)
            {
                float score = EvalCell(cell, coverCells, playerPositions);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = cell;
                }
            }
            
            return bestMove;
        }

        // Gets available Cells based on current position, does not include obstacle cells or cells with enemies 
        private List<Cell> GetAvailableCells() 
        {
            // Data.MovementRange
            return null;
        }

        // Generates score for each cell depending on sime heuristics 
        private float EvalCell(Cell cell, List<Cell> coverCells, List<Cell> playerPositions)
        {
            float score = 0;

            // Prefer cover
            if (IsCover(cell, coverCells)) score += 10f;

            // Avoid flanking
            score -= 10f * IsFlanked(cell, playerPositions);

            // Ensure attack chance
            score += 5f * HasLineOfSight(cell, playerPositions);

            return score;
        }

        private bool IsCover(Cell cell, List<Cell> coverCells)
        {
            // Placeholder logic for determining if a cell provides cover
            return coverCells.Contains(cell);
        }

        private int IsFlanked(Cell cell, List<Cell> playerPositions)
        {
            // Placeholder logic to check how many allies are flanking the enemy
            return 0;
        }

        private int HasLineOfSight(Cell cell, List<Cell> playerPositions)
        {
            // Placeholder logic getting numbers of allies in line of sight
            return 0;
        }
    }
}
