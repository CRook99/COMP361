using System;
using Controller;
using Managers;
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
        private bool _cursorLocked;

        public event Action<Cell> OnHoveredCellChanged;

        private void Awake()
        {
            _rayMask = LayerMask.GetMask("Cell");
            _cam = Camera.main;
        }
        
        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPause, LockCursor);
            EventManager.Subscribe(EventTypes.OnUnpause, UnlockCursor);
        }
        
        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPause, LockCursor);
            EventManager.Unsubscribe(EventTypes.OnUnpause, UnlockCursor);
        }

        private void Start()
        {
            _playerInput = InputManager.Instance.PlayerInput;
            _playerInput.Combat.Select.performed += OnSelectTile;
        }

        private void Update()
        {
            if (_cursorLocked) return;
            
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
                
                Cell newCell = TacticsGrid.Instance.GetCell(coordinate);
                if (newCell == null || newCell == _currentCell) return;
                
                _currentCell = newCell;
                OnHoveredCellChanged?.Invoke(_currentCell);
                if (!cursor.activeSelf) cursor.SetActive(true);
                cursor.transform.position = _currentCell.Position.ToVector3XZ(0.5f);
            }
            else
            {
                _currentCell = null;
                if (cursor.activeSelf) cursor.SetActive(false);
            }
        }

        private void OnSelectTile(InputAction.CallbackContext context)
        {
            if (_currentCell == null)
                Debug.Log("No cell hovered");
            else if (!_currentCell.Walkable)
                Debug.Log("Unwalkable cell selected");
            else
                ActiveAllyController.ActiveAlly.TryMoveToCell(_currentCell);
        }

        private void LockCursor()
        {
            _cursorLocked = true;
        }

        private void UnlockCursor()
        {
            _cursorLocked = false;
        }
    }
}