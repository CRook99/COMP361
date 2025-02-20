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
    private int _turnNumber = 0;
    // Getters for the gameManager -> Serialize
    public bool IsAllyTurn => _isAllyTurn; 
    public int TurnNumber => _turnNumber;

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
        _turnNumber++;
        Debug.Log("Ally's Turn");
    }

    public void StartEnemyTurn() 
    {
        EventManager.TriggerEvent(EventTypes.OnStartEnemyTurn);
        // Make UI element indicating whose turn it is subscribe to this

        _isAllyTurn = false;
        Debug.Log("Enemy's Turn");
    }

     public void SetTurnNumber(int turn)
    {
        _turnNumber = turn;
    }

    public void Autosave(){

    }

}