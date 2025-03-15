using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World
{
    public static class Pathfinder
    {
        private const float Sqrt2 = 1.4142f;

        public static List<Cell> FindPath(Cell start, Cell end)
        {
            if (start == null)
            {
                Debug.LogWarning("WARNING: Tried to pathfind with a null start cell");
                return new List<Cell>();
            }

            if (end == null)
            {
                Debug.LogWarning("WARNING: Tried to pathfind with a null end cell");
                return new List<Cell>();
            }
            
            var openSet = new List<Cell> { start };
            var closedSet = new HashSet<Cell>();

            var gScore = new Dictionary<Cell, float> { [start] = 0 };
            var fScore = new Dictionary<Cell, float> { [start] = Heuristic(start, end) };
            var parentMap = new Dictionary<Cell, Cell>();

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(cell => fScore[cell]).First();

                if (current.Position == end.Position)
                {
                    return ReconstructPath(parentMap, current);
                }

                openSet.Remove(current);
                closedSet.Add(current);

                for (int i = 0; i < 8; i++)
                {
                    var neighbour = current.Neighbours[i];

                    if (neighbour == null || !neighbour.Walkable || closedSet.Contains(neighbour))
                        continue;

                    if (IsDiagonalMove(i) && !IsDiagonalMoveValid(current, i))
                        continue;

                    float movementCost = IsDiagonalMove(i) ? Sqrt2 : 1;
                    float tentativeGScore = gScore[current] + movementCost;
                    
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else if (tentativeGScore >= gScore.GetValueOrDefault(neighbour, float.MaxValue))
                        continue;

                    parentMap[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                    fScore[neighbour] = gScore[neighbour] + Heuristic(neighbour, end);
                }
            }

            return new List<Cell>();
        }
        
        public static HashSet<Cell> FindReachableCells(Cell start, int range, bool walkable)
        {
            if (start == null)
            {
                Debug.LogWarning("Tried to find reachable area with null start cell");
                return new HashSet<Cell>();
            }

            if (range <= 0)
            {
                Debug.Log("Tried to find reachable area with non-positive range");
                return new HashSet<Cell>();
            }

            var reachable = new Dictionary<Cell, float>();
            var queue = new Queue<(Cell cell, float distance)>();
            
            queue.Enqueue((start, 0f));
            reachable[start] = 0f;

            float rangeFloat = range;

            while (queue.Count > 0)
            {
                var (current, currentDistance) = queue.Dequeue();

                if (currentDistance > rangeFloat || currentDistance > reachable[current])
                    continue;

                for (int i = 0; i < 8; i++)
                {
                    Cell neighbour = current.Neighbours[i];

                    if (neighbour == null)
                        continue;
                    
                    if (walkable && neighbour is not { Walkable: true })
                        continue;
                    
                    if (IsDiagonalMove(i) && !IsDiagonalMoveValid(current, i))
                        continue;

                    float newDistance = currentDistance + (IsDiagonalMove(i) ? Sqrt2 : 1f);

                    if (newDistance > rangeFloat)
                        continue;

                    if (reachable.TryGetValue(neighbour, out float existingDistance))
                    {
                        if (newDistance >= existingDistance)
                            continue;
                    }
                    
                    queue.Enqueue((neighbour, newDistance));
                    reachable[neighbour] = newDistance;
                }
            }

            return new HashSet<Cell>(reachable.Keys);
        }

        private static List<Cell> ReconstructPath(Dictionary<Cell, Cell> parentMap, Cell current)
        {
            List<Cell> path = new List<Cell> { current };

            while (parentMap.ContainsKey(current))
            {
                current = parentMap[current];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }

        private static float Heuristic(Cell a, Cell b)
        {
            float dx = Mathf.Abs(a.Position.x - b.Position.x);
            float dy = Mathf.Abs(a.Position.y - b.Position.y); 
            return Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        }

        private static bool IsDiagonalMove(int i)
        {
            return i % 2 == 1;
        }

        private static bool IsDiagonalMoveValid(Cell cell, int goalIndex)
        {
            Cell leftNeighbour = cell.Neighbours[goalIndex - 1];
            Cell rightNeighbour = cell.Neighbours[(goalIndex + 1) % 8];

            return leftNeighbour is { Walkable: true } && rightNeighbour is { Walkable: true };
        }
    }
}
