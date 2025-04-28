using System;
using System.Collections.Generic;
using System.Linq;
using Config;

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
        public bool CanUse;
        public int CurrentCooldown { get; set; }
        public int MaxCooldown { get; }

        public ActionState(int maxCooldown)
        {
            CanUse = true;
            CurrentCooldown = 0;
            MaxCooldown = maxCooldown;
        }

        public bool HasCooldown => MaxCooldown != 0;

        public void Use()
        {
            if (HasCooldown) CurrentCooldown = MaxCooldown;
            CanUse = false;
        }
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

        public Dictionary<ActionType,int> GetAllCooldowns() =>
            _actionMap.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.CurrentCooldown
            );

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

                if (state.HasCooldown)
                {
                    if (!state.CanUse)
                    {
                        state.CurrentCooldown--;
                        OnCooldownChanged?.Invoke(type, state.CurrentCooldown);
                    }
                    
                    state.CanUse = state.CurrentCooldown <= 0;
                }
                else
                {
                    state.CanUse = true;
                }
                
                if (state.CanUse)
                {
                    OnRefreshAction?.Invoke(type);
                }
            }
        }

         public void SetCooldown(ActionType type, int cooldown)
        {
            if (_actionMap.TryGetValue(type, out var state))
            {
                state.CurrentCooldown = cooldown;
                state.CanUse = (cooldown <= 0);
                OnCooldownChanged?.Invoke(type, cooldown);
                if (state.CanUse)
                    OnRefreshAction?.Invoke(type);
            }
        }

        public void RefreshAll()
        {
            foreach (var kvp in _actionMap)
            {
                var type  = kvp.Key;
                var state = kvp.Value;

                if (state.HasCooldown && state.CurrentCooldown > 0)
                    OnCooldownChanged?.Invoke(type, state.CurrentCooldown);

                if (state.CanUse)
                    OnRefreshAction?.Invoke(type);
            }
        }
    }
}