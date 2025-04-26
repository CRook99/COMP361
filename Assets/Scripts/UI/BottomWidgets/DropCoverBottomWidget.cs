using System;
using Controller;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
    public class DropCoverBottomWidget : SubBottomWidget
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button fireButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Space] [SerializeField] private DropCoverManager dropCoverManager;

        protected override void Awake()
        {
            base.Awake();

            backButton.onClick.AddListener(OnClickBackButton);
            fireButton.onClick.AddListener(OnClickFireButton);
        }

        private void Start()
        {
            if (dropCoverManager == null)
            {
                dropCoverManager = FindObjectOfType<DropCoverManager>();

                if (dropCoverManager == null)
                {
                    Debug.LogWarning("Drop cover manager was not found for DropCoverBottomWidget");
                    return;
                }
            }

            titleText.text = "Drop Cover";
            descriptionText.text = "Drop a block of full cover on the selected cell";
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
                HintManager.Instance.Hint("Can't drop cover here!", HintLevel.Warning);
                return;
            }

            dropCoverManager.HandleDropCover(target);
        }
    }
}