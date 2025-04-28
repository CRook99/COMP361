using System.Collections.Generic;
using Config;
using DG.Tweening;
using Entities;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AllyStatusWidget : MonoBehaviour
    {
        private const float ExpandScale = 1.2f; // Scale to which the widget expands when active
        private const float SwitchTime = 0.25f; // Time for a widget to expand/revert on switch
        
        private Ally _ally;
        private bool _active;
        private Dictionary<ActionType, AllyStatusActionWidget> _actionMap;

        [Header("Health")]
        [SerializeField] private Image healthFill;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Actions")] 
        [SerializeField] private AllyStatusActionWidget actionWidgetPrefab;
        [SerializeField] private Transform actionsParent;
        [SerializeField] private CanvasGroup actionsGroup;
        
        [Header("Misc")]
        [SerializeField] private Image fade;
        [SerializeField] private Transform scaleParent;
    
        private void Awake()
        {
            EventManager.Subscribe(EventTypes.OnActiveAllyChanged, ChangeState);
        }

        public void Initialize(Ally ally, List<ActionScriptableObject> actions)
        {
            _ally = ally;
            _ally.OnHealthChanged += UpdateHealth;
            _ally.Actions.OnUseAction += UseAction;
            _ally.Actions.OnRefreshAction += RefreshAction;

            _actionMap = new();
            
            foreach (ActionScriptableObject action in actions)
            {
                AllyStatusActionWidget widget = Instantiate(actionWidgetPrefab, actionsParent);
                widget.Data = action;

                if (action.Type == ActionType.Ability && ally.ChosenAbility != null)
                {
                    widget.SetIcon(ally.ChosenAbility.image);
                }
                else 
                {
                    widget.SetIcon(action.Icon);
                }

                _actionMap.Add(action.Type, widget);
            }
            
            UpdateHealth(_ally.CurrentHealth);
        }

        private void OnDisable()
        {
            _ally.OnHealthChanged -= UpdateHealth;
            _ally.Actions.OnUseAction -= UseAction;
            _ally.Actions.OnRefreshAction -= RefreshAction;

        }

        private void ChangeState(object data)
        {
            if (data is not Ally ally) return;

            if (ally == _ally) // Changed to this widget's ally
            {
                _active = true;
                scaleParent.DOScale(ExpandScale, SwitchTime);
                fade.DOFade(1f, SwitchTime);
                actionsGroup.DOFade(0f, SwitchTime);
            }
            else // Changed to another ally
            {
                if (!_active) return; // This widget was already not active
                
                _active = false;
                scaleParent.DOScale(1f, SwitchTime);
                fade.DOFade(0f, SwitchTime);
                actionsGroup.DOFade(1f, SwitchTime);
            }
        }

        private void UpdateHealth(int newHealth)
        {
            healthFill.fillAmount = (float)newHealth / _ally.Data.MaxHealth;
            healthText.text = newHealth.ToString();
        }

        private void UseAction(ActionType action)
        {
            _actionMap[action].Deactivate();
        }

        private void RefreshAction(ActionType action)
        {
            _actionMap[action].Activate();
        }
    }
}
