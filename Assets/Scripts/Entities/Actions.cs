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
    
    /**
     * Grouping class for a set of actions available to something
     */
    public class Actions
    {
        private readonly Dictionary<ActionType, bool> _actionMap;
        
        public event Action<ActionType> OnUseAction;
        public event Action<ActionType> OnRefreshAction;

        public Actions(List<ActionScriptableObject> availableActions)
        {
            _actionMap = availableActions
                .Select(a => a.Type)
                .ToDictionary(t => t, _ => true);
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