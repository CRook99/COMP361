using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyStatusWidget : MonoBehaviour
{
    private Ally _ally;
    private bool _active;

    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI healthText;
    
    private void Awake()
    {
        EventManager.Subscribe(EventTypes.OnActiveAllyChanged, ChangeState);
    }

    public void Initialize(Ally ally)
    {
        _ally = ally;
        _ally.OnHealthChanged += UpdateHealth;
    }

    private void ChangeState(object data)
    {
        if (data is not Ally ally) return;

        if (ally == _ally)
        {
            _active = true;
            // Changed to this widget's ally
        }
        else
        {
            _active = false;
            // Changed to another ally
        }
    }

    private void UpdateHealth(int newHealth)
    {
        healthFill.fillAmount = (float)newHealth / _ally.Data.MaxHealth;
        healthText.text = newHealth.ToString();
    }
}
