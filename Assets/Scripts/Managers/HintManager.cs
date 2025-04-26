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
        Warning,
        Error
    }

    public enum TutorialSteps
    {
        Movement,
        Allies,
        Shooting,
        Ability,
        DropCover,
        Airstrike,
        Enemies,
        None
    }

    [System.Serializable]
    public class TutorialItem
    {
        public TutorialSteps Step;
        public TutorialSteps NextStep;
        [TextArea] public string Message;
        [HideInInspector] public bool Fulfilled;
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
            { HintLevel.Warning, Color.yellow},
            { HintLevel.Error, Color.red }
        };

        [SerializeField] private List<TutorialItem> _tutorial;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
            
            ActivateTutorial(TutorialSteps.Movement);
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

        public void ActivateTutorial(TutorialSteps step)
        {
            TutorialItem item = _tutorial.Find(i => i.Step == step);
            if (item == null) return;
            if (item.Fulfilled)
            {
                ClearHint();
                return;
            }
            Hint(item.Message, HintLevel.Normal, persistent:true);
        }

        public void FulfilTutorial(TutorialSteps step)
        {
            TutorialItem item = _tutorial.Find(i => i.Step == step);
            if (item == null) return;
            
            item.Fulfilled = true;
            ClearHint();

            if (item.NextStep != TutorialSteps.None)
            {
                ActivateTutorial(item.NextStep);
            }
        }

        public void ClearHint()
        {
            CanvasGroup.alpha = 0f;
        }
    }
}