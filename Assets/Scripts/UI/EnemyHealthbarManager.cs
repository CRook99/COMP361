using System.Collections.Generic;
using Controller;
using UnityEngine;
using Entities;
using Managers;

public class EnemyHealthbarManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyHealthbarPrefab; 
    [SerializeField] private Transform uiCanvas; 

    private Dictionary<Enemy, EnemyHealthbar> _enemyHealthbars = new();
    private EnemyHealthbar _targetedHealthbar;

    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnSpawnEnemyLoadHealthBar, OnEnemySpawned);
        EventManager.Subscribe(EventTypes.OnEnemyKilled, OnEnemyKilled);
        EventManager.Subscribe(EventTypes.OnCycleTarget, OnCycleTarget);
        EventManager.Subscribe(EventTypes.OnPlayerConfirmShot, HideActive);
        EventManager.Subscribe(EventTypes.OnPlayerEndAiming, HideActive);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnSpawnEnemyLoadHealthBar, OnEnemySpawned);
        EventManager.Unsubscribe(EventTypes.OnEnemyKilled, OnEnemyKilled);
        EventManager.Unsubscribe(EventTypes.OnCycleTarget, OnCycleTarget);
        EventManager.Unsubscribe(EventTypes.OnPlayerConfirmShot, HideActive);
        EventManager.Unsubscribe(EventTypes.OnPlayerEndAiming, HideActive);

    }

    private void OnEnemySpawned(object data)
    {
        if (data is not Enemy enemy) return;
        
        // Instantiate health bar UI
        GameObject healthbarObj = Instantiate(enemyHealthbarPrefab, uiCanvas);
        EnemyHealthbar healthbar = healthbarObj.GetComponent<EnemyHealthbar>();

        // Add to dictionary (needs to be done first otherwise Initialize() returns early for some reason)
        _enemyHealthbars[enemy] = healthbar;

        // Initialize health bar with enemy reference
        healthbar.Initialize(enemy);
    }

    private void OnEnemyKilled(object data)
    {
        if (data is not Enemy enemy || !_enemyHealthbars.ContainsKey(enemy)) return;

        // Destroy the health bar UI
        Destroy(_enemyHealthbars[enemy].gameObject);

        // Remove from dictionary
        _enemyHealthbars.Remove(enemy);
    }

    private void OnCycleTarget(object data)
    {
        if (data is not TargetData targetData) return;

        _enemyHealthbars.TryGetValue(targetData.Target, out var healthbar);

        if (healthbar != null)
        {
            if (_targetedHealthbar != null) _targetedHealthbar.HideTargetingInfo();
            healthbar.ShowTargetingInfo(targetData.Cover);
            _targetedHealthbar = healthbar;
        }
    }

    private void HideActive()
    {
        if (_targetedHealthbar == null) return;
        
        _targetedHealthbar.HideTargetingInfo();
    }
}
