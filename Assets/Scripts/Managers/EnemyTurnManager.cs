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
            _enemies ??= new List<Enemy>(GameManager.Enemies);
            
            foreach (Enemy enemy in _enemies)
            {
                // If there is ally to shoot, it will always do that first
                if(enemy.ThereIsAllyToShoot()) {
                    yield return RunEnemyShootingAction(enemy);
                    yield return RunEnemyMovingAction(enemy);
                } else {
                    yield return RunEnemyMovingAction(enemy);
                    yield return RunEnemyShootingAction(enemy);
                }
            }

            TurnManager.Instance.EndEnemyTurn();
            _enemies = null;
        }

        private IEnumerator RunEnemyMovingAction(Enemy enemy)
        {
            // Movement
            Cell bestMove = enemy.GetBestMove();

            // Means enemy wants to move 
            if(bestMove != null && bestMove != enemy.CurrentCell) {
                yield return enemy.MoveToCell(bestMove);
            }
        }

        private IEnumerator RunEnemyShootingAction(Enemy enemy) {
            (Cell cell, Ally ally) = enemy.FindClosestVisibleAllyToShoot();

            // If ally null, then enemy will not shoot  
            if (enemy.IsDisarmed()) yield return null;
            else if (ally != null && cell != null)
            {
                yield return ShotManager.Instance.FireShotEnumerator(enemy, ally, cell);
            }
        }
    }
}