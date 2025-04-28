using System.Collections.Generic;
using UnityEngine;
using Controller;
using Entities;
using Managers;
using UnityEngine.UI;
using Utility.Serialization;


public class TurnManager : MonoBehaviour, IGameSerializable
{
  public static TurnManager Instance { get; private set; }

  [SerializeField] private Button endTurnButton; // Button for player to manually end their turn

  private bool _isAllyTurn = true;
  private int _turnNumber = 0;
  private HashSet<Ally> _actedAllies = new HashSet<Ally>();

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

    _isAllyTurn = true;
    _turnNumber++;
  }

  [ContextMenu("Enemy Turn")]
  public void StartEnemyTurn()
  {
    EventManager.TriggerEvent(EventTypes.OnStartEnemyTurn);
    // Make UI element indicating whose turn it is subscribe to this

    _isAllyTurn = false;

  }

  public void EndEnemyTurn()
  {
    EventManager.TriggerEvent(EventTypes.OnEndEnemyTurn);
    GameState.Instance.SaveGameState(overwriteSave:true);
    StartAllyTurn();
  }

  public bool IsAllyTurn()
  {
    return _isAllyTurn;
  }

  public void SetTurnNumber(int turn)
  {
    _turnNumber = turn;
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

  // --- IGameSerializable Implementation ---
  public bool Validate()
  {
    return _turnNumber >= 0;
  }

  public string Serialize()
  {
    TurnDTO dto = new TurnDTO
    {
      isAllyTurn = _isAllyTurn,
      turnNumber = _turnNumber
    };
    return JsonUtility.ToJson(dto, true);
  }

  public void Deserialize(string json)
  {
    TurnDTO dto = JsonUtility.FromJson<TurnDTO>(json);
    _isAllyTurn = dto.isAllyTurn;
    _turnNumber = dto.turnNumber;
  }
}