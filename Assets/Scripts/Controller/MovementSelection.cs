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
    public class MovementSelection : SelectionComponent
    {

        protected override void Awake()
        {
            base.Awake();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnSelectTile(InputAction.CallbackContext context)
        {
            if (ModeSwitcher.CurrentMode != ControlMode.Selection || _currentCell == null || !_currentCell.Walkable) return;
            
            ActiveAllyController.ActiveAlly.TryMoveToCell(_currentCell);
        }
    }
}