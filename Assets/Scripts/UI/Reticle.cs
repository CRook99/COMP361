using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    [SerializeField] private Image outerCircle;
    [SerializeField] private Image innerCircle;
    [SerializeField] private float rotationDuration;

    private Sequence _spinSequence;
    private Vector3 rotation = new Vector3(0, 0, 360);

    private void Awake()
    {
        _spinSequence = DOTween.Sequence();

        _spinSequence.Join(outerCircle.rectTransform
            .DORotate(rotation, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental));
        
        _spinSequence.Join(innerCircle.rectTransform
            .DORotate(-rotation, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental));

        _spinSequence.Pause();
    }

    private void OnEnable()
    {
        _spinSequence.Play();
    }

    private void OnDisable()
    {
        _spinSequence.Pause();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector3 screenPosition)
    {
        transform.position = screenPosition;
        gameObject.SetActive(true);
    }
}
