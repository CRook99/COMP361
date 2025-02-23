using System;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;
using World;


namespace Managers
{

    public class EnemyTurnManager : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnStartEnemyTurn, HandleEnemyTurn);
        }

        private void OnDisable()
        {
            // For some reason, event is getting un
            EventManager.Unsubscribe(EventTypes.OnStartEnemyTurn, HandleEnemyTurn);
        }

        [ContextMenu("Enemy Turn")]
        public void HandleEnemyTurn()
        {
            foreach (Enemy enemy in GameManager.Enemies)
            {
                Debug.Log("Running enemy turn");
                Cell bestMove = enemy.GetBestMove();
                enemy.TryMoveToCell(bestMove);
            }
        }
    }
}