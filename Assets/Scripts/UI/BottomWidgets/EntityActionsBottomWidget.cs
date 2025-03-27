using Controller;
using Entities;
using Managers;

namespace UI.BottomWidgets
{
    public class EntityActionsBottomWidget : ActionsBottomWidget
    {
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
        
        private void RefreshWidgets(object data)
        {
            if (data is not Ally activeAlly) return;

            foreach (var item in _actionMap)
            {
                if (activeAlly.Actions.CanUseAction(item.Key) && !item.Value.Active)
                {
                    item.Value.Activate();
                }
                else if (!activeAlly.Actions.CanUseAction(item.Key) && item.Value.Active)
                {
                    item.Value.Deactivate();
                }
            }

            _actionMap[ActionType.Ability].AbilityData = activeAlly.ChosenAbility;
            _actionMap[ActionType.Ability].RefreshGraphics();
        }
    }
}