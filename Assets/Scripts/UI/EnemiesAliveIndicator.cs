using UnityEngine;
using Managers;
using TMPro;
using System.Collections;
using System;

namespace UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class EnemiesAliveIndicator : MonoBehaviour
    {
        /* -------- internals -------- */
        private TMP_Text counter;

        private void Awake()
        {
            counter = GetComponent<TMP_Text>();
            UpdateLabel(0);
        }

        private void OnEnable()  => EventManager.Subscribe(EventTypes.OnUpdateEnemyCount, ShowEnemyCount);
        private void OnDisable() => EventManager.Unsubscribe(EventTypes.OnUpdateEnemyCount, ShowEnemyCount);

        private void ShowEnemyCount(object data)
        {
            if (data is not int newCount)
            {
                Debug.LogWarning("Passed incorrect type to EnemiesAliveIndicator::ShowEnemyCount");
                return;
            }

            UpdateLabel(newCount);
        }

        /* -------------- helpers -------------- */

        private void UpdateLabel(int value) {
            counter.text = $"Enemies Alive: {value}";
            counter.color = Color.blue;
        }
    }
}
