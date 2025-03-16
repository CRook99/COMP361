using Entities;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
    public class WeaponBottomWidget : SubBottomWidget
    {
        [TextArea(3, 5)]
        [SerializeField] private string bodyText;
        [SerializeField] private TextMeshProUGUI textElement;

        [Space]
        [SerializeField] private Button backButton;
        [SerializeField] private Button fireButton;

        protected override void Awake()
        {
            base.Awake();

            backButton.onClick.AddListener(OnClickBackButton);
            fireButton.onClick.AddListener(OnClickFireButton);
        }

        public override void Open()
        {
            if (!_playerReferences.ActiveAllyController.ActiveAlly.Actions.CanUseAction(ActionType.Weapon)) return;
            
            base.Open();
            
            EventManager.TriggerEvent(EventTypes.OnPlayerBeginAiming);
        }

        private void OnClickBackButton()
        {
            EventManager.TriggerEvent(EventTypes.OnPlayerEndAiming);
            BottomWidgetManager.Instance.Show(EBottomWidget.Movement);
        }

        private void OnClickFireButton()
        {
            EventManager.TriggerEvent(EventTypes.OnPlayerConfirmShot);
            EventManager.TriggerEvent(EventTypes.OnPlayerEndAiming);
        }
        
        public override bool CanOpen()
        {
            return _playerReferences.ActiveAllyController.ActiveAlly.Actions.CanUseAction(ActionType);
        }
    }
}