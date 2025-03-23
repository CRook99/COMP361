using System;
using System.Collections;
using Controller;
using DG.Tweening;
using Entities;
using UI.BottomWidgets;
using UnityEngine;
using Utility;
using World;

namespace Managers
{
    public class DropCoverManager : MonoBehaviour
    {
        public float DropDelay;
        public float DropTime;

        private Transform _camera;

        public GameObject CoverPrefab;

        private void Awake()
        {
            _camera = Camera.main.transform;
        }

        public void HandleDropCover(Cell target)
        {
            StartCoroutine(DropSequence(target));
        }

        private IEnumerator DropSequence(Cell target)
        {
            InputManager.Instance.PlayerInput.Disable();
            BottomWidgetManager.Instance.Show(EBottomWidget.AirSupportBase);
            AirSupportManager.Instance.Actions.UseAction(ActionType.DropCover);
            EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.DropCover);
            
            yield return new WaitForSeconds(DropDelay / 2);

            Vector3 spawnPoint = _camera.position + (_camera.right * 5f) + (_camera.up * (-1 * 5f));
            Vector3 dropPoint = target.Position.ToVector3XZ(1f);
            GameObject cover = Instantiate(CoverPrefab, spawnPoint, Quaternion.identity);

            cover.transform.DOMove(dropPoint, DropTime)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    InputManager.Instance.PlayerInput.Enable();

                    target.Walkable = false;
                    TacticsGrid.Instance.AddCover(target, CoverTypes.FullCover);
                });
        }
    }
}