using Controller;
using Entities;
using Managers;

namespace UI.BottomWidgets
{
    public class EntityActionsBottomWidget : ActionsBottomWidget
    {
        private Ally _currentAlly;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            EventManager.Subscribe(EventTypes.OnActiveAllyChanged, RefreshWidgets);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            EventManager.Unsubscribe(EventTypes.OnActiveAllyChanged, RefreshWidgets);
        }
        
        protected override void RefreshWidgets(object data)
        {
            if (data is not Ally activeAlly) return;

            if (_currentAlly != null)
                _currentAlly.Actions.OnCooldownChanged -= OnCooldownChanged;
            activeAlly.Actions.OnCooldownChanged += OnCooldownChanged;
            _currentAlly = activeAlly;

            foreach (var kvp in _actionMap)
            {
                ActionType type = kvp.Key;
                PrimaryActionWidget widget = kvp.Value;
                
                widget.UpdateCooldown(activeAlly.Actions.GetCooldown(type));
                if (activeAlly.Actions.CanUseAction(type) && !widget.Active)
                {
                    widget.Activate();
                }
                else if (!activeAlly.Actions.CanUseAction(type) && widget.Active)
                {
                    widget.Deactivate();
                }
                
                widget.UpdateCooldown(_currentAlly.Actions.GetCooldown(type));
            }

            _actionMap[ActionType.Ability].AbilityData = activeAlly.ChosenAbility;
            _actionMap[ActionType.Ability].RefreshGraphics();
        }

        public override void Open()
        {
            base.Open();
            
            EventManager.TriggerEvent(EventTypes.OnPlayerChangeMode, ControlMode.StandardMove);
        }
    }
}