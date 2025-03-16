using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
    public class AirstrikeBottomWidget : SubBottomWidget
    {
        [Space]
        [SerializeField] private Button backButton;

        protected override void Awake()
        {
            base.Awake();

            backButton.onClick.AddListener(OnClickBackButton);
        }

        public override bool CanOpen()
        {
            return _playerReferences.ActiveAllyController.ActiveAlly.Actions.CanUseAction(ActionType);
        }

        private void OnClickBackButton()
        {
            BottomWidgetManager.Instance.Show(EBottomWidget.AirSupportBase);
        }
    }
}