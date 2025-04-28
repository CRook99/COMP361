using System.Collections;
using System.Collections.Generic;
using Controller;
using Entities;
using Managers;
using UnityEngine;
using UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static List<Ally> Allies;
    public static List<Enemy> Enemies;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    
        Instance = this;
        DontDestroyOnLoad(gameObject);
    
        Allies = new List<Ally>();
        Enemies = new List<Enemy>();
    }

    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnSpawnAlly, AddAlly);
        EventManager.Subscribe(EventTypes.OnAllyFallen, RemoveAlly);
        EventManager.Subscribe(EventTypes.OnSpawnEnemyStartGame, AddEnemy);
        EventManager.Subscribe(EventTypes.OnEnemyKilled, RemoveEnemy);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnSpawnAlly, AddAlly);
        EventManager.Unsubscribe(EventTypes.OnAllyFallen, RemoveAlly);
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

    public void RemoveAlly(object data)
    {
        if (data is not Ally ally) return;
        Allies.Remove(ally);

        if (Allies.Count == 0)
        {
            StartCoroutine(EndSequence("You lost..."));
        }
    }

    public void RemoveEnemy(object data) {
        if (data is not Enemy enemy) return;
        Enemies.Remove(enemy);

        UpdateEnemiesAliveCounter();
        if (Enemies.Count == 0)
        {
            StartCoroutine(EndSequence("You won!"));
        }
    }

    // Helper function to update enemies alive counter
    private void UpdateEnemiesAliveCounter() {
        EventManager.TriggerEvent(EventTypes.OnUpdateEnemyCount, Enemies.Count);
    }

    private IEnumerator EndSequence(string message)
    {
        InputManager.Instance.PlayerInput.Disable();
        yield return new WaitForSeconds(1f);

        SceneParams.ResultsMessage = message;
        SceneManager.LoadSceneAsync("Results");
    }

    private void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        Allies?.Clear();
        Enemies?.Clear();
    }

}
