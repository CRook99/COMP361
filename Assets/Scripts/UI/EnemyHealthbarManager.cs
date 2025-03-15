using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;

public class EnemyHealthbarManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyHealthbarPrefab; 
    [SerializeField] private Transform uiCanvas; 

    private Dictionary<Enemy, EnemyHealthbar> _enemyHealthbars = new();

    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnSpawnEnemyLoadHealthBar, OnEnemySpawned);
        EventManager.Subscribe(EventTypes.OnEnemyKilled, OnEnemyKilled);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnSpawnEnemyLoadHealthBar, OnEnemySpawned);
        EventManager.Unsubscribe(EventTypes.OnEnemyKilled, OnEnemyKilled);
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
}
