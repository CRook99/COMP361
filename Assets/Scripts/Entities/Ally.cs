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
    }
}