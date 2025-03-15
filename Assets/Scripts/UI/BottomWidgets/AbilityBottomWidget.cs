using Entities;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
    public class AbilityBottomWidget : BottomWidget
    {
        [SerializeField] private string titleText;
        [SerializeField] private TextMeshProUGUI titleTextElement;
        [TextArea(3, 5)]
        [SerializeField] private string bodyText;
        [SerializeField] private TextMeshProUGUI bodyTextElement;

        [Space]
        [SerializeField] private Button backButton;

        protected override void Awake()
        {
            base.Awake();

            backButton.onClick.AddListener(OnClickBackButton);
            ActionType = ActionType.Ability;
        }

        public override void Open()
        {
            if (!_playerReferences.ActiveAllyController.ActiveAlly.Actions.CanUseAction(ActionType.Ability)) return;

            base.Open();

            EventManager.TriggerEvent(EventTypes.OnPlayerBeginAbility);
        }
        
        public override void Close()
        {
            base.Close();

            EventManager.TriggerEvent(EventTypes.OnPlayerEndAbility);
        }

        private void OnClickBackButton()
        {
            BottomWidgetManager.Instance.Show(EBottomWidget.Movement);
        }
    }
}