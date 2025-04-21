using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public enum HintLevel
    {
        Normal,
        Error,
    }
    
    public class HintManager : MonoBehaviour
    {
        private const float DefaultDuration = 3f;
        private const float FadeDuration = 0.5f;
        
        public static HintManager Instance;
        public CanvasGroup CanvasGroup;
        public TextMeshProUGUI Text;

        private readonly Dictionary<HintLevel, Color> _levelMap = new()
        {
            { HintLevel.Normal, Color.white },
            { HintLevel.Error, Color.yellow }
        };
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
            
            Hint("Move by Right Clicking a cell\nTry to stay in cover!", HintLevel.Normal, persistent:true);
        }

        public void Hint(string message, HintLevel level, float duration = DefaultDuration, bool persistent = false)
        {
            DOTween.KillAll();
            
            Text.text = message;
            Text.color = _levelMap[level];

            CanvasGroup.alpha = 1f;

            if (persistent)
                return;

            DOTween.Sequence()
                .AppendInterval(duration)
                .Append(CanvasGroup.DOFade(0f, FadeDuration).SetEase(Ease.OutQuad))
                .OnComplete(() => CanvasGroup.alpha = 0f);
        }
    }
}