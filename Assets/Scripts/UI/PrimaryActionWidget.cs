using System;
using DG.Tweening;
using Entities;
using TMPro;
using UI.BottomWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PrimaryActionWidget : MonoBehaviour
    {
        private const float SwitchTime = 0.25f;
        
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectTransform inactivePosition;
        [SerializeField] private RectTransform activePosition;
        [SerializeField] private Image bar;
        [SerializeField] private Button button;
        [SerializeField] private Color inactiveColor;
        
        public ActionScriptableObject Data;
        public AbilityScriptableObject AbilityData; // Will be null for non-abilities

        private bool _active;
        public bool Active => _active;

        private void Start()
        {
            icon.sprite = Data.Icon;
            text.text = Data.DisplayName;
            _active = true;

            if (Data.Type == ActionType.Move)
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                button.onClick.AddListener(() => BottomWidgetManager.Instance.Show(Data.WidgetType));
            }
        }

        public void Activate()
        {
            bar.DOFade(1f, 0f).SetEase(Ease.Linear);
            icon.rectTransform.DOAnchorPosY(activePosition.anchoredPosition.y, SwitchTime).SetEase(Ease.Linear);
            icon.color = Color.white;
            _active = true;
        }
        
        public void Deactivate()
        {
            bar.DOFade(0f, 0f).SetEase(Ease.Linear);
            icon.rectTransform.DOAnchorPosY(inactivePosition.anchoredPosition.y, SwitchTime).SetEase(Ease.Linear);
            icon.color = inactiveColor;
            _active = false;
        }

        // Only to be called by ability widget
        public void RefreshGraphics()
        {
            if (AbilityData == null) return; // TODO Try to get rid of this
            
            text.text = AbilityData.title;
            icon.sprite = AbilityData.image;
        }
    }
}