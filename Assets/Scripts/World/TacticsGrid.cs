using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace World
{
    // represents the game map
    //
    public class TacticsGrid : MonoBehaviour
    {
        public static TacticsGrid Instance { get; private set; }

        public Vector2Int InspectCell;

        private Dictionary<Vector2Int, Cell> _cellMap; // cells of game map
        private Dictionary<Vector2Int, Cell> _coverCells; // dict of obstacles on the map 

        private MapParser _mapParser;

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

            _cellMap = new Dictionary<Vector2Int, Cell>();
            _coverCells = new Dictionary<Vector2Int, Cell>();
            _mapParser = GetComponent<MapParser>();

            _mapParser.ReadMap(out _cellMap, out _coverCells);
            PrecomputeNeighbours();
        }

        public Cell GetCell(int x, int y)
        {
            if (x < 0 || y < 0) return null;

            _cellMap.TryGetValue(new Vector2Int(x, y), out var cell);
            return cell;
        }

        public Cell GetCell(Vector2Int xy)
        {
            if (xy.x < 0 || xy.y < 0) return null;

            _cellMap.TryGetValue(xy, out var cell);
            return cell;
        }

        // Gets current obstacles in the game world
        public HashSet<Cell> GetCoverCells()
        {
            return LinqUtility.ToHashSet(_coverCells.Values);
        }


        // Gets all cells in the game world
        public HashSet<Cell> GetAllCells()
        {
            return LinqUtility.ToHashSet(_cellMap.Values);
        }

        // Given start and end cells, checks if an obstacle exists somewhere between them
        // i.e a line drawn from end to start intersects with a obstacle tile
        // uses Bresenham's algorithm
        public bool ObstacleBetweenCells(Cell start, Cell end)
        {
            int x0 = start.Position.x;
            int y0 = start.Position.y;
            int dx = Math.Abs(end.Position.x - x0);
            int dy = Math.Abs(end.Position.y - y0);
            int sx = (x0 < end.Position.x) ? 1 : -1;
            int sy = (y0 < end.Position.y) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                Cell checkCell = Instance.GetCell(x0, y0);
                if (checkCell != null && checkCell.Cover == CoverTypes.FullCover)
                    return true;

                if (x0 == end.Position.x && y0 == end.Position.y)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return false;
        }

        private void PrecomputeNeighbours()
        {
            int[] xMap = { 0, 1, 1, 1, 0, -1, -1, -1 };
            int[] yMap = { 1, 1, 0, -1, -1, -1, 0, 1 };
            foreach (Cell cell in _cellMap.Values)
            {
                for (int i = 0; i < 8; i++)
                {
                    Cell neighbour = GetCell(cell.Position.x + xMap[i], cell.Position.y + yMap[i]);
                    if (neighbour != null)
                        cell.Neighbours[i] = neighbour;
                }
            }
        }

        public void AddCover(Cell cell, CoverTypes cover)
        {
            if (cell == null || !_cellMap.Values.Contains(cell) || _coverCells.Values.Contains(cell)) return;

            cell.Walkable = false;
            cell.Cover = cover;
            _coverCells.Add(cell.Position, cell);
        }

        // Checks if cell is as cover next to it
        public bool IsCover(Cell cell)
        {
            if (cell == null) throw new NullReferenceException("Cell is null");
        
            HashSet<Cell> obstacles = Instance.GetCoverCells();
            return
            obstacles.Contains(cell.N) ||
            obstacles.Contains(cell.S) ||
            obstacles.Contains(cell.W) ||
            obstacles.Contains(cell.E);
        }

        [ContextMenu("Inspect Cell")]
        private void Inspect()
        {
            Debug.Log(_cellMap[InspectCell]);
        }
    }
}