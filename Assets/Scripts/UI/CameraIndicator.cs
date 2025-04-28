using DG.Tweening;
using Managers;
using UnityEngine;

public class CameraIndicator : MonoBehaviour
{
    public RectTransform Image;

    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnCameraModeChanged, Change);
    }
    
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnCameraModeChanged, Change);
    }

    private void Change(object data)
    {
        if (data is not CameraMode mode) return;

        DOTween.KillAll();

        if (mode == CameraMode.Standard)
        {
            Image.DORotate(new Vector3(0f, 0f, 0f), 0.5f).SetEase(Ease.OutQuad);
        }
        else
        {
            Image.DORotate(new Vector3(0f, 0f, -90f), 0.5f).SetEase(Ease.OutQuad);
        }
    }
}
