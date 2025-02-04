using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace World
{
    // represents the game map
    //
    public class TacticsGrid : MonoBehaviour
    {
        public static TacticsGrid Instance { get; private set; }
        
        private Dictionary<Vector2Int, Cell> _cellMap; // cells of game map
        private Dictionary<Vector2Int, Cell> _obstacles; // dict of obstacles on the map 

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
            _obstacles = new Dictionary<Vector2Int, Cell>();
            _mapParser = GetComponent<MapParser>();
            
            _mapParser.ReadMap(out _cellMap, out _obstacles);
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
        public HashSet<Cell> GetObstacleCells() {
            return _obstacles.Values.ToHashSet();
        }


        // Gets all cells in the game world
        public HashSet<Cell> GetAllCells()
        {
            return _cellMap.Values.ToHashSet();
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

        private void OnDrawGizmos()
        {
            if (_cellMap == null) return;
            
            foreach (Cell cell in _cellMap.Values)
            {
                var pos = new Vector3(cell.Position.x, 1f, cell.Position.y);
                Handles.Label(pos, cell.Position.ToString());
            }
        }

    }
}