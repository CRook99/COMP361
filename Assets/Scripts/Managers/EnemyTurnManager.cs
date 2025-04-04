using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;

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
            
            _enemies = new List<Enemy>(GameManager.Enemies);
            
            // We subscribe to OnStartEnemyTurn so don't trigger it here
            foreach (Enemy enemy in _enemies)
            {
                yield return RunEnemyAction(enemy);
            }
            
            foreach (Enemy enemy in _enemies)
            {
                yield return RunEnemyShootingAction(enemy);
            }

            TurnManager.Instance.EndEnemyTurn();
        }

        private IEnumerator RunEnemyAction(Enemy enemy)
        {
            // Movement
            Cell bestMove = enemy.GetBestMove();
            yield return enemy.MoveToCell(bestMove);
        }

        private IEnumerator RunEnemyShootingAction(Enemy enemy) {
            (Cell cell, Ally ally) = enemy.FindClosestVisibleAllyToShoot();
            if (ally != null)
            {
                yield return new WaitForSeconds(0.25f);

                Cell enemycell = enemy.CurrentCell;
                if(enemycell != cell) yield return enemy.MoveToCell(cell);
                
                ShotManager.Instance.FireShot(enemy, ally);
                yield return new WaitForSeconds(0.25f);

                if(enemycell != cell) yield return enemy.MoveToCell(enemycell);
            }
        }
    }
}