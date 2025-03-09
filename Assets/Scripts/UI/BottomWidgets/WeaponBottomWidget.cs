using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
    public class WeaponBottomWidget : BottomWidget
    {
        [TextArea(3, 5)]
        [SerializeField] private string bodyText;
        [SerializeField] private TextMeshProUGUI textElement;

        [Space]
        [SerializeField] private Button backButton;

        protected override void Awake()
        {
            base.Awake();

            backButton.onClick.AddListener(OnClickBackButton);
        }

        public override void Open()
        {
            base.Open();
            
            EventManager.TriggerEvent(EventTypes.OnPlayerBeginAiming);
        }

        public override void Close()
        {
            base.Close();
            
            EventManager.TriggerEvent(EventTypes.OnPlayerEndAiming);
        }

        private void OnClickBackButton()
        {
            BottomWidgetManager.Instance.Show(EBottomWidget.Movement);
        }
    }
}