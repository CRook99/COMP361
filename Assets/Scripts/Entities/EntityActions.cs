using System;
using System.Collections.Generic;
using System.Linq;

namespace Entities
{
    public enum ActionType
    {
        Move,
        Weapon,
        Ability,
        DropCover,
        Airstrike
    }
    
    public class EntityActions
    {
        private readonly Dictionary<ActionType, bool> _actionMap;
        
        public event Action<ActionType> OnUseAction;
        public event Action<ActionType> OnRefreshAction;

        public EntityActions()
        {
            _actionMap = Enum.GetValues(typeof(ActionType))
                .Cast<ActionType>()
                .ToDictionary(type => type, _ => true);
        }

        public void UseAction(ActionType type)
        {
            _actionMap[type] = false;
            OnUseAction?.Invoke(type);
        }

        public bool CanUseAction(ActionType type)
        {
            return _actionMap[type];
        }
        
        public void Refresh()
        {
            foreach (var key in _actionMap.Keys.ToList())
            {
                _actionMap[key] = true;
                OnRefreshAction?.Invoke(key);
            }
        }
    }
}