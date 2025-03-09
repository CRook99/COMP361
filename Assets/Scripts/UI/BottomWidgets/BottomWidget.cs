using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    
        protected virtual void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _height = _rect.rect.height;

            _rect.anchoredPosition = new Vector2(0f, -_height);
        }
    
        public void Show()
        {
            _rect.DOAnchorPosY(0f, ChangeTime).SetEase(Ease.OutQuad);
        }
    
        public void Hide()
        {
            _rect.DOAnchorPosY(-_height, ChangeTime).SetEase(Ease.OutQuad);
        }
    }
}
