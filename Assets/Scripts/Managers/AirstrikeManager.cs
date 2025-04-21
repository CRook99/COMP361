using System;
using System.Collections;
using Controller;
using DG.Tweening;
using Entities;
using Managers;
using UI.BottomWidgets;
using UnityEngine;
using Utility;

namespace Managers
{
    public class AirstrikeManager : MonoBehaviour
    {
        public float StrikeDelay;
        public float StrikeTime;
        public int StrikeDamage;

        private Transform _camera;
        
        public GameObject MissilePrefab;

        private void Awake()
        {
            _camera = Camera.main.transform;
        }

        // Assumes target is not null
        public void HandleAirstrike(Cell target)
        {
            StartCoroutine(AirstrikeSequence(target));
        }

        private IEnumerator AirstrikeSequence(Cell target)
        {
            InputManager.Instance.PlayerInput.Disable();
            BottomWidgetManager.Instance.Show(EBottomWidget.AirSupportBase);
            AirSupportManager.Instance.Actions.UseAction(ActionType.Airstrike);
            EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.Airstrike);
            
            yield return new WaitForSeconds(StrikeDelay / 2);

            Vector3 spawnPoint = _camera.position + (_camera.right * 5f) + (_camera.up * (-1 * 5f));
            Vector3 strikePoint = target.Position.ToVector3XZ();
            Missile missile = Instantiate(MissilePrefab, spawnPoint, Quaternion.LookRotation(strikePoint - spawnPoint)).GetComponent<Missile>();

            missile.transform.DOMove(strikePoint, StrikeTime)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    foreach (Enemy enemy in GameManager.Enemies)
                    {
                        if (enemy.CurrentCell == target)
                        {
                            enemy.TakeDamage(StrikeDamage);
                            break;
                        }
                    }

                    missile.TrailParticleSystem.transform.parent = null;
                    missile.TrailParticleSystem.Stop();
                    missile.ExplosionParticleSystem.transform.parent = null;
                    missile.ExplosionParticleSystem.Play();
                    
                    Destroy(missile.gameObject);
                    // Play VFX/SFX
                    InputManager.Instance.PlayerInput.Enable();
                });
        }
    }
}