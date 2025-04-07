using System.Collections.Generic;
using Entities;
using Managers;

namespace UI.BottomWidgets
{
    public class AirSupportActionsBottomWidget : ActionsBottomWidget
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            
            EventManager.Subscribe(EventTypes.OnEndEnemyTurn, RefreshWidgets);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            
            EventManager.Unsubscribe(EventTypes.OnEndEnemyTurn, RefreshWidgets);
        }
        
        protected override void RefreshWidgets(object data)
        {
            AirSupportManager.Instance.Actions.Refresh();
            foreach (ActionType action in _actionMap.Keys)
            {
                if (AirSupportManager.Instance.Actions.CanUseAction(action))
                {
                    _actionMap[action].Activate();
                }
                else
                {
                    _actionMap[action].Deactivate();
                }
            }
        }
    }
}