using System;
using System.Collections;
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
            EventManager.Subscribe(EventTypes.OnStartEnemyTurn, EnterEnemyTurn);
        }
        
        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnStartEnemyTurn, EnterEnemyTurn);
        }

        // Void method to expose to events
        private void EnterEnemyTurn()
        {
            StartCoroutine(HandleEnemyTurn());
        }
        
        public IEnumerator HandleEnemyTurn()
        {
            // make sure camera is in standard mode before running enemy turn
            // TODO Can't trigger event here as the cam controller is solely responsible for calling this.
            // TODO Introduce new event OnForceCameraMode and subscribe to this in CameraController?
            // EventManager.TriggerEvent(EventTypes.OnCameraModeChanged, CameraMode.Standard);
            
            _enemies ??= new List<Enemy>(GameManager.Enemies);
            
            if (_enemies.Count > 0)
            {
                // We subscribe to OnStartEnemyTurn so don't trigger it here
                foreach (Enemy enemy in _enemies)
                {
                    yield return RunEnemyAction(enemy);
                }
                EventManager.TriggerEvent(EventTypes.OnEndEnemyTurn);
            }
            else
            {
                _enemies = null;
                EventManager.TriggerEvent(EventTypes.OnEndEnemyTurn);
            }
        }

        private IEnumerator RunEnemyAction(Enemy enemy)
        {
            // Movement
            Cell bestMove = enemy.GetBestMove();
            yield return enemy.MoveToCell(bestMove);
            
            // Shooting
            (Cell cell, Ally ally) = enemy.FindClosestVisibleAllyToShoot();
            if (ally != null)
            {
                // TODO: Implement shooting logic
                // yield return shooty shooty
                Debug.Log($"{enemy.name} is shooting at {ally.name}");
            }
        }
    }
}