using System;
using Controller;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using World;

namespace Controller
{
    public class TileSelect{
    public Cell CursorToTile(Camera cam, LayerMask rayMask) {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mousePosition);

            Cell newCell = null;
            if (Physics.Raycast(ray, out var hit, 100f, rayMask))
            {
                Vector3 position = hit.transform.position;
                Vector2Int coordinate = new Vector2Int
                (
                    Mathf.RoundToInt(position.x),
                    Mathf.RoundToInt(position.z)
                );
                
                newCell = TacticsGrid.Instance.GetCell(coordinate);
            }

            return newCell;
    }
}
}