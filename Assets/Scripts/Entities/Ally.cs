using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using World;

namespace Entities
{
    public class Ally : Entity
    {
        private MoveArea _moveArea;
        private HashSet<Cell> _reachableCells;
        
        protected override void Awake()
        {
            base.Awake();

            _moveArea= GetComponentInChildren<MoveArea>();
        }

        protected override void Start()
        {
            base.Start();
            
            EventManager.TriggerEvent(EventTypes.OnSpawnAlly, this);
        }

        protected void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnStartAllyTurn, OnStartTurn);
            EventManager.Subscribe(EventTypes.OnActiveAllyChanged, OnActiveAllyChanged);
        }

        public override void TryMoveToCell(Cell destination)
        {
            if (!Actions.CanUseAction(ActionType.Move) || !_reachableCells.Contains(destination))
            {
                // Handle unable
                return;
            }
            
            EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.Move);
            _moveArea.Hide();
            MoveToCell(destination);
        }

        protected override IEnumerator FollowPath(List<Cell> path)
        {
            EventManager.TriggerEvent(EventTypes.OnPlayerBeginMove, this);
            yield return base.FollowPath(path);
            EventManager.TriggerEvent(EventTypes.OnPlayerEndMove);
        }

        private void OnActiveAllyChanged(object data)
        {
            if (data is not Ally ally) return;
            if (ally == this)
            {
                if (Actions.CanUseAction(ActionType.Move))
                {
                    _moveArea.Show();
                }
            }
            else
            {
                _moveArea.Hide();
            }
        }

        private void OnStartTurn()
        {
            _reachableCells = Pathfinder.FindReachableCells(CurrentCell, Data.MovementRange);
            _moveArea.GenerateMesh(_reachableCells, CurrentCell.Position);
        }
    }
}