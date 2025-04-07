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
    public abstract class SelectionComponent : PlayerComponent
    {
        [SerializeField] private GameObject cursor;
        
        protected Camera _cam;
        protected Ray _ray;
        protected Cell _currentCell;
        private PlayerInput _playerInput;
        protected LayerMask _rayMask;
        protected bool _cursorLocked;
        

        public event Action<Cell> OnHoveredCellChanged;

        protected virtual void Awake()
        {
            _rayMask = LayerMask.GetMask("Cell");
            _cam = Camera.main;
        }
        
        protected virtual void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPause, DisableCursor);
            EventManager.Subscribe(EventTypes.OnUnpause, EnableCursor);
            EventManager.Subscribe(EventTypes.OnPlayerChangeMode, OnPlayerChangeMode);
        }
        
        protected virtual void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnPause, DisableCursor);
            EventManager.Unsubscribe(EventTypes.OnUnpause, EnableCursor);
            EventManager.Unsubscribe(EventTypes.OnPlayerChangeMode, OnPlayerChangeMode);
        }

        protected virtual void Start()
        {
            _playerInput = InputManager.Instance.PlayerInput;
            _playerInput.Combat.Select.performed += OnSelectTile;
        }

        protected virtual void Update()
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

        protected abstract void OnSelectTile(InputAction.CallbackContext context);

        private void EnableCursor()
        {
            cursor.SetActive(true);
            _cursorLocked = false;
        }
        
        private void DisableCursor()
        {
            cursor.SetActive(false);
            _cursorLocked = true;
        }

        private void ToggleCursor(bool b)
        {
            if (b) EnableCursor();
            else DisableCursor();
        }

        private void OnPlayerChangeMode(object data)
        {
            if (data is not ActionType mode) return;
            
            ToggleCursor(mode == ActionType.Move);
        }
    }
}