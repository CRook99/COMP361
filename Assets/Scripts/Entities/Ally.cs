using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Entities
{
    public class Ally : Entity
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            
            EventManager.TriggerEvent(EventTypes.OnSpawnAlly, this);
        }

        public override void TryMoveToCell(Cell destination)
        {
            if (!Actions.CanUseAction(ActionType.Move))
            {
                // Handle unable
                return;
            }
            
            EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.Move);
            MoveToCell(destination);
        }
    }
}