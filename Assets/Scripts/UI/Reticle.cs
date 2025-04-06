using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    [SerializeField] private Image outerCircle;
    [SerializeField] private Image innerCircle;
    [SerializeField] private float rotationDuration;
    
    private RectTransform _outerCircleRect;
    private RectTransform _innerCircleRect;
    private float _rotationFactor;

    private void Awake()
    {
        _outerCircleRect = outerCircle.rectTransform;
        _innerCircleRect = innerCircle.rectTransform;
        _rotationFactor = 360f * (1 / rotationDuration);
    }

    private void Update()
    {
        _outerCircleRect.Rotate(Vector3.forward, _rotationFactor * Time.deltaTime);
        _innerCircleRect.Rotate(Vector3.forward, -_rotationFactor * Time.deltaTime);
    }

    public void Show()
    {
        gameObject.SetActive(true);
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
