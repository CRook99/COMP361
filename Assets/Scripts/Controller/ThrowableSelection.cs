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
    public class ThrowableSelection : SelectionComponent
    {
        private ThrowableScriptableObject Throwable;
        private MoveArea _throwAOE;

        protected override void Awake()
        {
            base.Awake();
        }
        
        public ThrowableSelection(ThrowableScriptableObject t)
        {
            Throwable = t;
            _currentCell = ActiveAllyController.ActiveAlly.CurrentCell;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ActiveAllyController.ActiveAlly.EnableThrow(Throwable.ThrowRadius);

            var _affectedCells = Pathfinder.FindReachableCells(_currentCell, Throwable.EffectRadius);
            _throwAOE.GenerateMesh(_affectedCells, _currentCell.Position);
            _throwAOE.Show();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            ActiveAllyController.ActiveAlly.DisableThrow();
            _throwAOE.Hide();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            var _affectedCells = Pathfinder.FindReachableCells(_currentCell, Throwable.EffectRadius);
            _throwAOE.GenerateMesh(_affectedCells, _currentCell.Position);
        }

        protected override void OnSelectTile(InputAction.CallbackContext context)
        {
            if (ModeSwitcher.CurrentMode != ControlMode.Selection || _currentCell == null || !_currentCell.Walkable) return;
            
            var _affectedCells = Pathfinder.FindReachableCells(_currentCell, Throwable.EffectRadius);
            ActiveAllyController.ActiveAlly.TryThrow(Throwable, _currentCell, _affectedCells);
        }
    }
}