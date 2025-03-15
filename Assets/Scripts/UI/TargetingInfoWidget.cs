using System;
using Entities;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TargetingInfoWidget : MonoBehaviour
    {
        [Header("Graphics")] 
        [SerializeField] private Image shieldImage;
        [SerializeField] private Sprite fullImage;
        [SerializeField] private Sprite halfImage;

        [Header("Text")] 
        [SerializeField] private TextMeshProUGUI chanceText;

        [Header("Positioning")] 
        [SerializeField] private Vector2 offset;
        [SerializeField] private GameObject toggleParent;

        private RectTransform _rect;
        private Camera _camera;
        private Transform _enemyTransform;

        private void Awake()
        {
            _camera = Camera.main;
            _rect = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnCycleTarget, OnCycleTarget);
            EventManager.Subscribe(EventTypes.OnPlayerBeginAiming, OnPlayerBeginAiming);
            EventManager.Subscribe(EventTypes.OnPlayerEndAiming, OnPlayerEndAiming);
        }

        private void Update()
        {
            if (_enemyTransform == null) return;
            
            transform.position = _camera.WorldToScreenPoint(_enemyTransform.position) + new Vector3(offset.x, offset.y, 0);
        }

        private void OnCycleTarget(object target)
        {
            if (target is not Enemy enemy) return;
            
            _enemyTransform = enemy.transform;
        }

        private void OnPlayerBeginAiming()
        {
            toggleParent.SetActive(true);
        }

        private void OnPlayerEndAiming()
        {
            _enemyTransform = null;
            toggleParent.SetActive(false);
        }
    }
}