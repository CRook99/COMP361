using System;
using System.Collections.Generic;
using Controller;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using World;

namespace Controller
{
    public class ThrowableSelection : SelectionComponent
    {
        public ThrowableScriptableObject Throwable;
        public MoveArea throwAOE;
        private bool _active;
        private HashSet<Cell> _throwableCells;

        protected override void Awake()
        {
            base.Awake();

            _throwableCells = new HashSet<Cell>();
        }
        
        // public ThrowableSelection(ThrowableScriptableObject t)
        // {
        //     Throwable = t;
        //     _currentCell = ActiveAllyController.ActiveAlly.CurrentCell;
        // }

        protected override void OnEnable()
        {
            base.OnEnable();
            //ActiveAllyController.ActiveAlly.EnableThrow(Throwable.ThrowRadius);
            
            EventManager.Subscribe(EventTypes.OnPlayerBeginAbility, Activate);
            EventManager.Subscribe(EventTypes.OnPlayerEndAbility, Deactivate);

            //var _affectedCells = Pathfinder.FindReachableCells(_currentCell, Throwable.EffectRadius);
            //_throwAOE.GenerateMesh(_affectedCells, _currentCell.Position);
            //_throwAOE.Show();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            // ActiveAllyController.ActiveAlly.DisableThrow();
            // _throwAOE.Hide();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            if (!_active) return;
            
            base.Update();

            if (!_throwableCells.Contains(_currentCell)) return;
            
            var _affectedCells = Pathfinder.FindReachableCells(_currentCell, Throwable.EffectRadius);
            if (_affectedCells.Count == 0) return;
            
            throwAOE.GenerateMesh(_affectedCells, _currentCell.Position);
            throwAOE.transform.position = _currentCell.Position.ToVector3XZ(0.1f);
        }

        private void Activate()
        {
            _active = true;
            
            ModeSwitcher.SwitchMode(ControlMode.Selection);
            ActiveAllyController.ActiveAlly.EnableThrow(Throwable.ThrowRadius, out _throwableCells); // Placeholder value
            throwAOE.Show();
        }

        private void Deactivate()
        {
            _active = false;
            
            ModeSwitcher.SwitchMode(ControlMode.StandardMove);
            ActiveAllyController.ActiveAlly.DisableThrow();
            throwAOE.Hide();
        }

        protected override void OnSelectTile(InputAction.CallbackContext context)
        {
            if (ModeSwitcher.CurrentMode != ControlMode.Selection || _currentCell == null || !_currentCell.Walkable) return;
            
            var _affectedCells = Pathfinder.FindReachableCells(_currentCell, Throwable.EffectRadius);
            ActiveAllyController.ActiveAlly.TryThrow(Throwable, _currentCell, _affectedCells);
        }
    }
}