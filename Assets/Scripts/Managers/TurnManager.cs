using System.Collections.Generic;
using UnityEngine;
using System;
using Entities;
using Managers;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;

public class TurnManager : MonoBehaviour 
{
    public static TurnManager Instance {get; private set;}
    
    private bool _isAllyTurn = true;

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
        StartAllyTurn();
    }

    public void StartAllyTurn() 
    {
        EventManager.TriggerEvent(EventTypes.OnStartAllyTurn);
        // Make UI element indicating whose turn it is subscribe to this

        _isAllyTurn = true;
        Debug.Log("Ally's Turn");
    }

    public void StartEnemyTurn() 
    {
        EventManager.TriggerEvent(EventTypes.OnStartEnemyTurn);
        // Make UI element indicating whose turn it is subscribe to this

        _isAllyTurn = false;
        Debug.Log("Enemy's Turn");

    }
}