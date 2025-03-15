using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
    public class AirstrikeBottomWidget : BottomWidget
    {
        [Space]
        [SerializeField] private Button backButton;

        protected override void Awake()
        {
            base.Awake();

            backButton.onClick.AddListener(OnClickBackButton);
        }

        private void OnClickBackButton()
        {
            BottomWidgetManager.Instance.Show(EBottomWidget.AirSupportBase);
        }
    }
}