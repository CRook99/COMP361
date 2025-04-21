using System;
using Controller;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
    public class AirstrikeBottomWidget : SubBottomWidget
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button fireButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Space]
        [SerializeField] private AirstrikeManager airstrikeManager;

        protected override void Awake()
        {
            base.Awake();

            backButton.onClick.AddListener(OnClickBackButton);
            fireButton.onClick.AddListener(OnClickFireButton);
        }

        private void Start()
        {
            if (airstrikeManager == null)
            {
                airstrikeManager = FindObjectOfType<AirstrikeManager>();

                if (airstrikeManager == null)
                {
                    Debug.LogWarning("Airstrike manager was not found for AirstrikeBottomWidget");
                    return;
                }
            }
            
            titleText.text = "Airstrike";
            descriptionText.text = $"Deliver a single-target airstrike dealing {airstrikeManager.StrikeDamage} damage";
        }

        public override bool CanOpen()
        {
            return AirSupportManager.Instance.Actions.CanUseAction(ActionType);
        }

        private void OnClickBackButton()
        {
            BottomWidgetManager.Instance.Show(EBottomWidget.AirSupportBase);
        }

        private void OnClickFireButton()
        {
            Cell target = AirSupportManager.Instance.GetHoveredCell();
            if (target == null)
            {
                HintManager.Instance.Hint("Can't airstrike here!", HintLevel.Error);
                return;
            }
            
            airstrikeManager.HandleAirstrike(target);
        }
    }
}