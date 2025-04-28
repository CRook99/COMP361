using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class DamageNumbers : MonoBehaviour
    {
        [SerializeField] private float time;
        [SerializeField] private Vector2 initialOffset;
        [SerializeField] private float verticalMovement;
        [SerializeField] private TextMeshProUGUI numberText;
        
        private CanvasGroup _canvasGroup;
        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
        }

        public void Animate(Vector3 position, int amount)
        {
            StartCoroutine(Cleanup());
            
            transform.SetAsLastSibling();
            
            Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
            screenPos += initialOffset;

            _rect.position = screenPos;
            _canvasGroup.alpha = 1f;
            numberText.text = amount.ToString();
            
            DOTween.Kill(_rect);
            DOTween.Kill(_canvasGroup);

            _rect.DOMove(screenPos + Vector2.up * verticalMovement, time).SetEase(Ease.Linear);
            _canvasGroup.DOFade(0f, time).SetEase(Ease.InExpo);
        }

        private IEnumerator Cleanup()
        {
            yield return new WaitForSeconds(time);
            _canvasGroup.alpha = 0f;
        }
    }
}

