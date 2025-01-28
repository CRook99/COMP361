using System.Collections.Generic;
using UnityEngine;
using System;
using Entities;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;

public class TurnManager : MonoBehaviour 
{

    public static TurnManager Instance {get; private set;}

    private Entity ally, enemy;
    private bool isAllyTurn = true;
    public event Action OnTurnStart, OnTurnEnd; 

    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start() 
    {
        ally = FindObjectOfType<Ally>();
        enemy = FindObjectOfType<Enemy>();

        if (ally == null || enemy == null) 
        {
            Debug.LogError("Ally or Enemy not found");
        }
        
        // Initialize ally and enemy ?

        StartTurn();
    }

    private void StartTurn() 
    {
        OnTurnStart?.Invoke(); 

        if (isAllyTurn) 
        {
            Debug.Log("Ally's Turn");
            ally.TakeTurn(() => EndTurn());
        }
        else 
        {
            Debug.Log("Enemy's Turn");
            enemy.TakeTurn(() => EndTurn());
        }
    }

    private void EndTurn() 
    {
        OnTurnEnd?.Invoke();
        // if (ally.IsAlive() && enemy.IsAlive()) 
        // {
        //     isAllyTurn = !isAllyTurn;
        //     StartTurn();
        // } 
        // else if (ally.IsAlive()) 
        // {
        //     Debug.Log("ALLY WINS");
        // }
        // else 
        // {
        //     Debug.Log("ENEMY WINS");
        // }
        isAllyTurn = !isAllyTurn;
        StartTurn();
    }
}