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

    public class ActionState
    {
        public int CurrentCooldown { get; set; }
        public int MaxCooldown { get; }
        
        public ActionState(int maxCooldown)
        {
            CurrentCooldown = 0;
            MaxCooldown = maxCooldown;
        }

        public bool CanUse => CurrentCooldown <= 0;
        public bool HasCooldown => MaxCooldown != 0;
        public void Use() => CurrentCooldown = MaxCooldown;
    }
    
    /**
     * Grouping class for a set of actions available to something
     */
    public class Actions
    {
        private readonly Dictionary<ActionType, ActionState> _actionMap;
        
        public event Action<ActionType> OnUseAction;
        public event Action<ActionType> OnRefreshAction;
        public event Action<ActionType, int> OnCooldownChanged;

        public Actions(List<ActionScriptableObject> availableActions)
        {
            _actionMap = availableActions
                .ToDictionary(
                    a => a.Type,
                    a => new ActionState(a.Cooldown)
                );
        }

        public void UseAction(ActionType type)
        {
            if (!_actionMap.TryGetValue(type, out var state))
                return;
            
            state.Use();
            if (state.HasCooldown)
                OnCooldownChanged?.Invoke(type, state.CurrentCooldown);
            
            OnUseAction?.Invoke(type);
        }

        public bool CanUseAction(ActionType type)
        {
            return _actionMap.TryGetValue(type, out var state) && state.CanUse;
        }

        public int GetCooldown(ActionType type)
        {
            if (_actionMap.TryGetValue(type, out var state))
            {
                return state.CurrentCooldown;
            }

            return -1;
        }
        
        public void Tick()
        {
            foreach (var kvp in _actionMap)
            {
                ActionType type = kvp.Key;
                ActionState state = kvp.Value;

                if (!state.CanUse)
                {
                    state.CurrentCooldown--;
                    OnCooldownChanged?.Invoke(type, state.CurrentCooldown);
                }
                
                if (state.CanUse)
                {
                    OnRefreshAction?.Invoke(type);
                }
            }
        }
    }
}