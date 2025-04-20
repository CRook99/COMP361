using Controller;
using Entities;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
    public class AbilityBottomWidget : SubBottomWidget
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

            AbilityScriptableObject ability = _playerReferences.ActiveAllyController.ActiveAlly.ChosenAbility;
            titleTextElement.text = ability.title;
            bodyTextElement.text = ability.description;
            EventManager.TriggerEvent(EventTypes.OnPlayerBeginAbility);
        }
        
        public override void Close()
        {
            base.Close();

            EventManager.TriggerEvent(EventTypes.OnPlayerEndAbility);
        }

        public override bool CanOpen()
        {
            return _playerReferences.ActiveAllyController.ActiveAlly.Actions.CanUseAction(ActionType);
        }

        private void OnClickBackButton()
        {
            BottomWidgetManager.Instance.Show(EBottomWidget.Movement);
        }
    }
}