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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(HandleEnemyTurn());
            }
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
                EventManager.TriggerEvent(EventTypes.OnStartEnemyTurn);
                foreach (Enemy enemy in _enemies)
                {
                    yield return RunEnemyAction(enemy);
                }
                EventManager.TriggerEvent(EventTypes.OnEndEnemyTurn);
            }
            else
            {
                _enemies = null;
                EventManager.TriggerEvent(EventTypes.OnEnemyEndMove);
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