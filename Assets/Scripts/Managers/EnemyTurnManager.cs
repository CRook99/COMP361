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
            // make sure camera is in standard mode before running enemy turn
            // TODO Can't trigger event here as the cam controller is solely responsible for calling this.
            // TODO Introduce new event OnForceCameraMode and subscribe to this in CameraController?
            EventManager.TriggerEvent(EventTypes.OnCameraModeChanged, CameraMode.Standard);

            foreach (Enemy enemy in GameManager.Enemies)
            {
                Cell bestMove = enemy.GetBestMove();
                enemy.TryMoveToCell(bestMove);

                Ally ally = enemy.FindClosestVisibleAllyToShoot();
                if (ally != null)
                {
                    // TODO: Implement shooting
                }
            }
        }
    }
}