using System;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;
using World;
using UnityEngine.SearchService;


namespace Managers
{

    public class EnemyTurnManager : MonoBehaviour
    {
        private List<Enemy> _enemies = null;

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnStartEnemyTurn, ChainEnemyMoves);
            EventManager.Subscribe(EventTypes.OnEnemyBeginMove, HandleEnemyTurn);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnStartEnemyTurn, ChainEnemyMoves);
            EventManager.Unsubscribe(EventTypes.OnEnemyBeginMove, HandleEnemyTurn);
        }

        public void ChainEnemyMoves() {
            _enemies ??= new List<Enemy>(GameManager.Enemies);

            // there is still enemies to move 
            if(_enemies.Count > 0) {
                Enemy enemy = _enemies[0];
                _enemies.RemoveAt(0);
                EventManager.TriggerEvent(EventTypes.OnEnemyBeginMove, enemy);
            } else {
                _enemies = null;
                EventManager.TriggerEvent(EventTypes.OnEnemyEndMove);
            }
        }   


        [ContextMenu("Enemy Turn")]
        public void HandleEnemyTurn(object obj)
        {
            if(obj is not Enemy enemy)
                throw new ArgumentException("HandleEnemyTurn called with non-Enemy object");

            // make sure camera is in standard mode before running enemy turn
            EventManager.TriggerEvent(EventTypes.OnCameraModeChanged, CameraMode.Standard);

            // foreach (Enemy enemy in GameManager.Enemies)
            {
                Cell bestMove = enemy.GetBestMove();
                enemy.TryMoveToCell(bestMove);
                
                // Ally ally = enemy.FindClosestVisibleAllyToShoot();
                // if (ally != null)
                // {
                //     // TODO: Implement shooting
                // }
            }
        }
    }
}