using Entities;
using UnityEngine;

namespace UI.BottomWidgets
{
    public class SubBottomWidget : BottomWidget
    {
        [SerializeField] protected ActionType ActionType;
        
        public override bool CanOpen()
        {
            return _playerReferences.ActiveAllyController.ActiveAlly.Actions.CanUseAction(ActionType);
        }
    }
}