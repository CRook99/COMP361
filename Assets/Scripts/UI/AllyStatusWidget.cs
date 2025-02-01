using System;
using System.Collections;
using System.Collections.Generic;
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

        [Header("Actions")] [SerializeField] private Transform actionsParent;
        
        [Header("Misc")]
        [SerializeField] private Image fade;
        [SerializeField] private Transform scaleParent;
    
        private void Awake()
        {
            EventManager.Subscribe(EventTypes.OnActiveAllyChanged, ChangeState);

            _actionMap = new();

            foreach (Transform child in actionsParent)
            {
                if (child == actionsParent) continue;

                AllyStatusActionWidget widget = child.GetComponent<AllyStatusActionWidget>();
                _actionMap.Add(widget.Data.Type, widget);
            }
        }

        public void Initialize(Ally ally)
        {
            _ally = ally;
            _ally.OnHealthChanged += UpdateHealth;
            _ally.OnUseAction += UseAction;
            _ally.OnRefreshAction += RefreshAction;
        }

        private void OnDisable()
        {
            _ally.OnHealthChanged -= UpdateHealth;
            _ally.OnUseAction -= UseAction;
            _ally.OnRefreshAction -= RefreshAction;

        }

        private void ChangeState(object data)
        {
            if (data is not Ally ally) return;

            if (ally == _ally) // Changed to this widget's ally
            {
                _active = true;
                scaleParent.DOScale(ExpandScale, SwitchTime);
                fade.DOFade(1f, SwitchTime);
            }
            else // Changed to another ally
            {
                if (!_active) return; // This widget was already not active
                
                _active = false;
                scaleParent.DOScale(1f, SwitchTime);
                fade.DOFade(0f, SwitchTime);
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
