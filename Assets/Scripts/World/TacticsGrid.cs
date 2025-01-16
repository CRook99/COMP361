using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace World
{
    public class TacticsGrid : MonoBehaviour
    {
        private Dictionary<Vector2Int, Cell> _cellMap;
        private Dictionary<Vector2Int, Cell> _obstacles;

        private MapParser _mapParser;

        private void Awake()
        {
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
    }
}