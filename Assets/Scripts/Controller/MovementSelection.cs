using Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using World;

namespace Controller
{
    public class MovementSelection : PlayerComponent
    {
        [SerializeField] private GameObject cursor;

        private Camera _cam;
        private Ray _ray;
        private Cell _currentCell;
        private PlayerInput _playerInput;
        private LayerMask _rayMask;

        private void Awake()
        {
            _rayMask = LayerMask.GetMask("Cell");
            _cam = Camera.main;
        }

        private void Start()
        {
            _playerInput = InputManager.Instance.PlayerInput;

            _playerInput.Combat.Select.performed += OnSelectTile;
        }

        private void Update()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            _ray = _cam.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(_ray, out var hit, 100f, _rayMask))
            {
                Vector3 position = hit.transform.position;
                Vector2Int coordinate = new Vector2Int
                (
                    Mathf.RoundToInt(position.x),
                    Mathf.RoundToInt(position.z)
                );
                
                _currentCell = TacticsGrid.Instance.GetCell(coordinate);
                if (_currentCell != null)
                {
                    if (!cursor.activeSelf) cursor.SetActive(true);
                    cursor.transform.position = _currentCell.Position.ToVector3XZ(0.5f);
                }
            }
            else
            {
                _currentCell = null;
                if (cursor.activeSelf) cursor.SetActive(false);
            }
        }

        private void OnSelectTile(InputAction.CallbackContext context)
        {
            if (_currentCell != null)
            {
                ActiveAllyController.ActiveAlly.MoveToCell(_currentCell);
            }
            else
            {
                Debug.Log("No cell hovered");
            }
        }
    }
}