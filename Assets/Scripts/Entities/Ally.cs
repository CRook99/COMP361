using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Entities
{
    public class Ally : Entity
    {
        public event Action<ActionType> OnUseAction;
        public event Action<ActionType> OnRefreshAction;
        
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            
            EventManager.TriggerEvent(EventTypes.OnSpawnAlly, this);
        }

        [ContextMenu("Test Use Move")]
        private void TestUseMove()
        {
            OnUseAction?.Invoke(ActionType.Move);
        }
        
        [ContextMenu("Test Use Weapon")]
        private void TestUseWeapon()
        {
            OnUseAction?.Invoke(ActionType.Weapon);
        }
        
        [ContextMenu("Test Refresh Move")]
        private void TestRefreshMove()
        {
            OnRefreshAction?.Invoke(ActionType.Move);
        }
    }
}