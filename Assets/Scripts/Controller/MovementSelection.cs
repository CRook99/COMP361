using UnityEngine.InputSystem;

namespace Controller
{
    public class MovementSelection : SelectionComponent
    {
        protected override void OnSelectTile(InputAction.CallbackContext context)
        {
            if (ModeSwitcher.CurrentMode != ControlMode.Move || _currentCell == null || !_currentCell.Walkable) return;

            ActiveAllyController.ActiveAlly.TryMoveToCell(_currentCell);
        }
    }
}