using System;
using System.Collections.Generic;
using UnityEngine;
using Entities;

namespace Managers
{

  public class EnemyTurnManager : MonoBehaviour
  {
    private List<Enemy> _enemies = null;

    private void OnEnable()
    {
      EventManager.Subscribe(EventTypes.OnStartEnemyTurn, ChainEnemyMoves);
      EventManager.Subscribe(EventTypes.OnEnemyBeginMove, HandleEnemyMoveSequence);
      EventManager.Subscribe(EventTypes.OnEnemyBeginShoot, HandleEnemyShooting);
    }

    private void OnDisable()
    {
      EventManager.Unsubscribe(EventTypes.OnStartEnemyTurn, ChainEnemyMoves);
      EventManager.Unsubscribe(EventTypes.OnEnemyBeginMove, HandleEnemyMoveSequence);
      EventManager.Unsubscribe(EventTypes.OnEnemyBeginShoot, HandleEnemyShooting);
    }


    [ContextMenu("Enemy Turn")]
    public void HandleEnemyTurn()
    {
      // make sure camera is in standard mode before running enemy turn
      // TODO Can't trigger event here as the cam controller is solely responsible for calling this.
      // TODO Introduce new event OnForceCameraMode and subscribe to this in CameraController?
      EventManager.TriggerEvent(EventTypes.OnCameraModeChanged, CameraMode.Standard);
      _enemies ??= new List<Enemy>(GameManager.Enemies);

      // There is still enemies to move 
      if (_enemies.Count > 0)
      {
        Enemy enemy = _enemies[0];
        _enemies.RemoveAt(0);
        EventManager.TriggerEvent(EventTypes.OnEnemyBeginMove, enemy);
      }
      else
      {
        _enemies = null;
        EventManager.TriggerEvent(EventTypes.OnEnemyEndMove);
      }
    }

    [ContextMenu("Enemy Turn")]
    private void HandleEnemyMoveSequence(object obj)
    {
      if (obj is not Enemy enemy)
        throw new ArgumentException("HandleEnemyTurn called with non-Enemy object");

      // Make sure camera is in standard mode before running enemy turn
      EventManager.TriggerEvent(EventTypes.OnCameraModeChanged, CameraMode.Standard);
      Cell bestMove = enemy.GetBestMove();

      // This will trigger event HandleEnemyShooting with event OnEnemyBeginShoot
      enemy.TryMoveToCell(bestMove);
    }

    private void HandleEnemyShooting(object obj)
    {
      if (obj is not Enemy enemy)
        throw new ArgumentException("HandleEnemyTurn called with non-Enemy object");

      Ally ally = enemy.FindClosestVisibleAllyToShoot();
      if (ally != null)
      {
        // TODO: Implement shooting logic
        Debug.Log($"{enemy.name} is shooting at {ally.name}");
      }

      // Goes to the next enemy move
      EventManager.TriggerEvent(EventTypes.OnStartEnemyTurn);
    }
  }
}