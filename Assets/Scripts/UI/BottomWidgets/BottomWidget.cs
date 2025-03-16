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

        public virtual void Open()
        {
            Show();
        }

        public virtual void Close()
        {
            Hide();
        }
    
        private void Show()
        {
            _rect.DOAnchorPosY(0f, ChangeTime).SetEase(Ease.OutQuad);
        }
    
        private void Hide()
        {
            _rect.DOAnchorPosY(-_height, ChangeTime).SetEase(Ease.OutQuad);
        }

        public abstract bool CanOpen();
    }
}
