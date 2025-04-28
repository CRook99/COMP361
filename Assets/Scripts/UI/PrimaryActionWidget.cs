using Config;
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
        [SerializeField] private GameObject cooldownPanel;
        [SerializeField] private TextMeshProUGUI cooldownText;
        
        public ActionScriptableObject Data;
        public AbilityScriptableObject AbilityData; // Will be null for non-abilities

        private bool _active;
        public bool Active => _active;

        private void Start()
        {
            icon.sprite = Data.Icon;
            text.text = Data.DisplayName;
            _active = true;

            if (cooldownPanel != null)
            {
                cooldownPanel.SetActive(false);
            }

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

            if (cooldownPanel != null)
            {
                cooldownPanel.SetActive(false);
            }
        }
        
        public void Deactivate()
        {
            bar.DOFade(0f, 0f).SetEase(Ease.Linear);
            icon.rectTransform.DOAnchorPosY(inactivePosition.anchoredPosition.y, SwitchTime).SetEase(Ease.Linear);
            icon.color = inactiveColor;
            _active = false;
        }

        public void UpdateCooldown(int cooldown)
        {
            if (cooldownPanel == null || cooldownText == null) return;
            
            if (cooldown > 0)
            {
                cooldownPanel.SetActive(true);
                cooldownText.text = cooldown.ToString();
                Deactivate();
            }
            else
            {
                cooldownPanel.SetActive(false);
            }
        }

        // Only to be called by ability widget
        public void RefreshGraphics()
        {
            if (AbilityData == null) return;
            
            text.text = AbilityData.title;
            icon.sprite = AbilityData.image;
        }
    }
}