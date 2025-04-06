using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.BottomWidgets
{
   public abstract class BottomWidget : MonoBehaviour
    {
        private const float ChangeTime = 0.25f;

        public EBottomWidget Type;
        
        private RectTransform _rect;
        private float _height;

        protected PlayerReferences _playerReferences;
    
        protected virtual void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _height = _rect.rect.height;

            _rect.anchoredPosition = new Vector2(0f, -_height);

            _playerReferences = FindObjectOfType<PlayerReferences>();
        }

        private Coroutine _moveRoutine;

        public virtual void Open()
        {
            StartMoveCoroutine(0f);
        }

        public virtual void Close()
        {
            StartMoveCoroutine(-_height);
        }

        private void StartMoveCoroutine(float targetY)
        {
            if (_moveRoutine != null)
                StopCoroutine(_moveRoutine);

            _moveRoutine = StartCoroutine(DoAnchorMove(targetY));
        }

        private IEnumerator DoAnchorMove(float targetY)
        {
            float elapsed = 0f;
            Vector2 startPos = _rect.anchoredPosition;
            Vector2 targetPos = new Vector2(startPos.x, targetY);

            while (elapsed < ChangeTime)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / ChangeTime);
                t = 1f - Mathf.Pow(1f - t, 2f); // quad out
                _rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }

            _rect.anchoredPosition = targetPos;
        }

        public abstract bool CanOpen();
    }
}
