using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Managers;
using UnityEngine;
using System.IO;


public class GameManager : MonoBehaviour
{
    public static List<Ally> Allies;
    public static List<Enemy> Enemies;
    
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        Allies = new();
        Enemies = new();
    }

    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnSpawnAlly, AddAlly);
        EventManager.Subscribe(EventTypes.OnSpawnEnemy, AddEnemy);

    }
    
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnSpawnAlly, AddAlly);
        EventManager.Unsubscribe(EventTypes.OnSpawnEnemy, AddEnemy);

    }

    public void AddAlly(object data)
    {
        if (data is not Ally ally) return;
        
        Allies.Add(ally);
    }

    public void AddEnemy(object data)
    {
        if (data is not Enemy enemy) return;
        
        Enemies.Add(enemy);
    }

    // Temporary method to demonstrate json serialization
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
    {
        SaveGameState();
    }
    }

    public void SaveGameState()
    {
        GameState state = new GameState
        {
            Allies = new List<AllyData>(),
            Enemies = new List<EnemyData>(),
            isAllyTurn = TurnManager.Instance != null ? TurnManager.Instance.IsAllyTurn : true // Default to ally turn if no instance
        };

        foreach (Ally ally in Allies)
        {
            AllyData data = new AllyData
            {
                posX = ally.transform.position.x,
                posY = ally.transform.position.y,
                posZ = ally.transform.position.z,
                currentHealth = ally.Health,
                entityDataName = ally.Data != null ? ally.Data.name : "Unknown"
            };

            state.Allies.Add(data);
        }

        foreach (Enemy enemy in Enemies)
        {
            EnemyData data = new EnemyData
            {
                posX = enemy.transform.position.x,
                posY = enemy.transform.position.y,
                posZ = enemy.transform.position.z,
                currentHealth = enemy.Health,
                entityDataName = enemy.Data != null ? enemy.Data.name : "Unknown"
            };

            state.Enemies.Add(data);
        }

        string json = state.Serialize();

        string folderPath = @"C:\Users\adminprx\Documents\Courses\SoftProject\project\COMP361\Assets\Scripts\Serialization";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = $"Save_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        string filePath = Path.Combine(folderPath, fileName);
        
        File.WriteAllText(filePath, json);

        Debug.Log("Game saved successfully to " + filePath);
    }

}
