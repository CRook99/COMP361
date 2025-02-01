using System;
using System.Collections;
using System.Collections.Generic;
using Managers;

namespace Entities
{
    public class Ally : Entity
    {
        protected override void Awake()
        {
            base.Awake();
            
            EventManager.TriggerEvent(EventTypes.OnSpawnAlly, this);
        }
    }
}