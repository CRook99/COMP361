using System.Collections.Generic;
using UnityEngine;
using System;
using Controller;
using Entities;
using Managers;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;


public class TurnManager : MonoBehaviour 
{
    public static TurnManager Instance {get; private set;}

    [SerializeField] private Button endTurnButton; // Button for player to manually end their turn
    
    private bool _isAllyTurn = true;
    private HashSet<Ally> _actedAllies= new HashSet<Ally>();

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

        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(StartEnemyTurn);
        }
        else
        {
            Debug.LogWarning("EndTurn button not passed to TurnManager");
        }
    }

    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnPlayerChangeMode, OnPlayerChangeMode);
    }
    
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnPlayerChangeMode, OnPlayerChangeMode);
    }

    private void Start() 
    {
        StartAllyTurn();
    }

    public void StartAllyTurn() 
    {
        // Reset actions
        _actedAllies.Clear();
        
        EventManager.TriggerEvent(EventTypes.OnStartAllyTurn);
        // Make UI element indicating whose turn it is subscribe to this

        _isAllyTurn = true;
        Debug.Log("Ally's Turn");
    }

    [ContextMenu("Enemy Turn")]
    public void StartEnemyTurn() 
    {
        EventManager.TriggerEvent(EventTypes.OnStartEnemyTurn);
        // Make UI element indicating whose turn it is subscribe to this

        _isAllyTurn = false;
        Debug.Log("Enemy's Turn");

    }

    public bool IsAllyTurn()
    {
        return _isAllyTurn;
    }

    public void RegisterAction(Ally unit)
    {
        if (!HasUnitActed(unit))
        {
            _actedAllies.Add(unit);
        }
    }

    public bool HasUnitActed(Ally unit)
    {
        return _actedAllies.Contains(unit);
    }

    private void OnPlayerChangeMode(object data)
    {
        if (data is not ControlMode mode)
        {
            Debug.LogWarning("Passed incorrect type to TurnManager::OnPlayerChangeMode");
            return;
        }

        if (endTurnButton == null) return;

        endTurnButton.interactable = mode != ControlMode.Selection;
    }
}