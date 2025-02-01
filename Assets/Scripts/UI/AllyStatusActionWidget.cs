using System;
using DG.Tweening;
using Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class AllyStatusActionWidget : MonoBehaviour
    {
        private const float SwitchTime = 0.5f;
        
        [SerializeField] private Image icon;
        [SerializeField] private RectTransform restingPosition;
        [SerializeField] private RectTransform activatedPosition;
        [SerializeField] private Image bar;
        
        public ActionScriptableObject Data;

        private void Awake()
        {
            icon.sprite = Data.Icon;
        }

        public void Activate()
        {
            bar.DOFade(1f, SwitchTime).SetEase(Ease.Linear);
            icon.rectTransform.DOMoveY(activatedPosition.position.y, SwitchTime).SetEase(Ease.Linear);
        }
        
        public void Deactivate()
        {
            bar.DOFade(0f, SwitchTime).SetEase(Ease.Linear);
            icon.rectTransform.DOMoveY(restingPosition.position.y, SwitchTime).SetEase(Ease.Linear);
        }
    }
}