using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Entities;
using Managers;
using UnityEngine;
using World;
using System.IO;
using UnityEngine.SceneManagement;
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
        EventManager.Subscribe(EventTypes.OnSpawnEnemyStartGame, AddEnemy);
        EventManager.Subscribe(EventTypes.OnEnemyKilled, RemoveEnemy);
    }
    
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnSpawnAlly, AddAlly);
        EventManager.Unsubscribe(EventTypes.OnSpawnEnemyStartGame, AddEnemy);
        EventManager.Unsubscribe(EventTypes.OnEnemyKilled, RemoveEnemy);
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
        UpdateEnemiesAliveCounter();
    }

    public void RemoveEnemy(object data) {
        if (data is not Enemy enemy) return;
        Enemies.Remove(enemy);

        UpdateEnemiesAliveCounter();
        if (Enemies.Count == 0)
        {
            StartCoroutine(VictorySequence());
        }
    }

    private void UpdateEnemiesAliveCounter() {
        EventManager.TriggerEvent(EventTypes.OnUpdateEnemyCount, Enemies.Count);
    }

    private IEnumerator VictorySequence()
    {
        Debug.Log($"All Enemies are dead!!!");
        InputManager.Instance.PlayerInput.Disable();
        yield return new WaitForSeconds(1f);
        
        // Show victory message
        
        SceneManager.LoadSceneAsync("Results");
    }

    // Temporary method to demonstrate json serialization
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)){
            //GameObject gameStateGO = new GameObject("GameState");
            //GameState gameState = gameStateGO.AddComponent<GameState>();
            //GameState.Instance.SaveGameState();
            GameState.Instance.LoadGameState(@"Assets\Scripts\Serialization\Save_File.json");
        }

        if (Input.GetKeyDown(KeyCode.T)){
            Debug.Log("Turn++");
            if (TurnManager.Instance.IsAllyTurn() == true){
                TurnManager.Instance.StartEnemyTurn();
            }else{
                TurnManager.Instance.StartAllyTurn();
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.TakeDamage(90);
            }
        }
    }

}
