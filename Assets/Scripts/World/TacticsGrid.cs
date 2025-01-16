using System;
using System.Collections.Generic;
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
        }

        public Cell GetCell(int x, int y)
        {
            return _cellMap[new Vector2Int(x, y)];
        }

        public Cell GetCell(Vector2Int xy)
        {
            return _cellMap[xy];
        }
    }
}