using System;
using UnityEngine;
using UnityEngine.UI;
using Entities;
using Managers;
using TMPro;

public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField] private Image healthFill; 
    [SerializeField] private TextMeshProUGUI healthText;
    
    // TARGETING
    [SerializeField] private GameObject targetingParent;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Sprite fullShield;
    [SerializeField] private Sprite halfShield;
    [SerializeField] private Sprite noShield;
    [SerializeField] private TextMeshProUGUI chanceText;
    
    private Transform _enemyTransform;
    private Camera _camera;
    private Enemy _enemy;

    public void Initialize(Enemy enemy)
    {
        _enemy = enemy;
        _enemyTransform = enemy.transform;
        _camera = Camera.main;
        healthText = GetComponentInChildren<TextMeshProUGUI>();

        _enemy.OnHealthChanged += UpdateHealth;
        
        UpdateHealth(_enemy.CurrentHealth);
    }

    private void OnDestroy()
    {
        if (_enemy != null)
        {
            _enemy.OnHealthChanged -= UpdateHealth;
        }
    }

    private void UpdateHealth(int newHealth)
    {
        healthFill.fillAmount = (float)newHealth / _enemy.Data.MaxHealth;
        healthText.text = $"{newHealth}";
    }

    private void LateUpdate()
    {
        if (_enemyTransform == null) return;

        Vector3 screenPos = _camera.WorldToScreenPoint(_enemyTransform.position + Vector3.up * 2f);
        transform.position = screenPos;
    }

    public void ShowTargetingInfo(CoverTypes cover)
    {
        switch (cover)
        {
            case CoverTypes.FullCover:
                shieldImage.sprite = fullShield;
                chanceText.text = "0%";
                break;
            case CoverTypes.HalfCover:
                shieldImage.sprite = halfShield;
                chanceText.text = "50%";
                break;
            case CoverTypes.NoCover:
                shieldImage.sprite = noShield;
                chanceText.text = "100%";
                break;
        }
        
        targetingParent.SetActive(true);
    }

    public void HideTargetingInfo()
    {
        targetingParent.SetActive(false);
    }
}
