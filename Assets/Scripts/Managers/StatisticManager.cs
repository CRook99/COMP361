using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers; 

public class StatisticsManager : MonoBehaviour
{
    private static StatisticsManager _instance;

    public static StatisticsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("StatisticsManager");
                _instance = go.AddComponent<StatisticsManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // Stats Variables (8)
    private int enemiesVanquished = 0;
    private int damageDealt = 0;
    private int shotsLanded = 0;
    private int spacesMoved = 0;
    private int fallenSoldiers = 0;
    private int damageReceived = 0;
    private int shotsTaken = 0;
    private int chanceShotsDodged = 0;

    // Subscribe
    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnEnemyKilled, OnEnemyKilled);
        EventManager.Subscribe(EventTypes.OnDamageDealt, OnDamageDealt);
        EventManager.Subscribe(EventTypes.OnShotLanded, OnShotLanded);
        EventManager.Subscribe(EventTypes.OnSpaceMoved, OnSpaceMoved);
        EventManager.Subscribe(EventTypes.OnAllyFallen, OnAllyFallen);
        EventManager.Subscribe(EventTypes.OnDamageTaken, OnDamageTaken);
        EventManager.Subscribe(EventTypes.OnShotTaken, OnShotTaken);
        EventManager.Subscribe(EventTypes.OnChanceShotDodged, OnChanceShotDodged);
    }

    // Unsubscribe
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnEnemyKilled, OnEnemyKilled);
        EventManager.Unsubscribe(EventTypes.OnDamageDealt, OnDamageDealt);
        EventManager.Unsubscribe(EventTypes.OnShotLanded, OnShotLanded);
        EventManager.Unsubscribe(EventTypes.OnSpaceMoved, OnSpaceMoved);
        EventManager.Unsubscribe(EventTypes.OnAllyFallen, OnAllyFallen);
        EventManager.Unsubscribe(EventTypes.OnDamageTaken, OnDamageTaken);
        EventManager.Unsubscribe(EventTypes.OnShotTaken, OnShotTaken);
        EventManager.Unsubscribe(EventTypes.OnChanceShotDodged, OnChanceShotDodged);
    }

    // Explicit event handler methods
    private void OnEnemyKilled(object data) => IncrementEnemiesVanquished();
    private void OnDamageDealt(object data) => AddDamageDealt((int)data);
    private void OnShotLanded(object data) => AddShotsLanded((int)data);
    private void OnSpaceMoved(object data) => AddSpacesMoved((int)data);
    private void OnAllyFallen(object data) => IncrementFallenSoldiers();
    private void OnDamageTaken(object data) => AddDamageTaken((int)data);
    private void OnShotTaken(object data) => AddShotsTaken((int)data);
    private void OnChanceShotDodged(object data) => AddChanceShotsDodged((int)data);



    private void IncrementEnemiesVanquished() => enemiesVanquished++;
    private void AddDamageDealt(int amount) => damageDealt += amount;
    private void AddShotsLanded(int count) => shotsLanded += count;
    private void AddSpacesMoved(int count) => spacesMoved += count;
    private void IncrementFallenSoldiers() => fallenSoldiers++;
    private void AddDamageTaken(int amount) => damageReceived += amount;
    private void AddShotsTaken(int count) => shotsTaken += count;
    private void AddChanceShotsDodged(int count) => chanceShotsDodged += count;


    public int GetEnemiesVanquished() => enemiesVanquished;
    public int GetDamageDealt() => damageDealt;
    public int GetShotsLanded() => shotsLanded;
    public int GetSpacesMoved() => spacesMoved;
    public int GetFallenSoldiers() => fallenSoldiers;
    public int GetDamageReceived() => damageReceived;
    public int GetShotsTaken() => shotsTaken;
    public int GetChanceShotsDodged() => chanceShotsDodged;


    public void ResetStats()
    {
    enemiesVanquished = 0;
    damageDealt = 0;
    shotsLanded = 0;
    spacesMoved = 0;
    fallenSoldiers = 0;
    damageReceived = 0;
    shotsTaken = 0;
    chanceShotsDodged = 0;
    }
    
}

    