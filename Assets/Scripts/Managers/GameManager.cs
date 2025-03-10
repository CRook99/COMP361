using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Entities;
using Managers;
using UnityEngine;
using World;
using System.IO;
using Utility.Serialization;

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
        if (Input.GetKeyDown(KeyCode.L)){
            //Debug.Log("L");

            GameObject gameStateGO = new GameObject("GameState");
            GameState gameState = gameStateGO.AddComponent<GameState>();
            gameState.SaveGameState();
            //gameState.LoadGameState(@"Assets\Scripts\Serialization\Save_File.json");
        }

        if (Input.GetKeyDown(KeyCode.T)){
            Debug.Log("Turn++");
            if (TurnManager.Instance.IsAllyTurn() == true){
                TurnManager.Instance.StartEnemyTurn();
            }else{
                TurnManager.Instance.StartAllyTurn();
            }
        }
    }

}
