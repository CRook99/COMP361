using DG.Tweening;
using Entities;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ActiveAllyIndicator : MonoBehaviour
    {
        private const float FadeTime = 0.1f;

        [SerializeField] private Sprite onScreenSprite;
        [SerializeField] private Sprite offScreenSprite;
        
        [SerializeField] private Vector3 standardOffset;
        [SerializeField] private Vector3 airSupportOffset;
        [SerializeField] private Vector3 allyOffset;
        [SerializeField] private float bobMagnitude;
        [SerializeField] private float bobSpeed;
        [SerializeField] [Range(0, 1)] private float marginFraction;

        private Transform _allyTransform;
        private Camera _camera;
        private Image _image;
        private Vector3 _offset;
        private float _margin;

        private bool _wasOffScreen;

        private void Awake()
        {
            _camera = Camera.main;
            _image = GetComponent<Image>();
            _offset = standardOffset;
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnActiveAllyChanged, OnActiveAllyChanged);
            EventManager.Subscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
            EventManager.Subscribe(EventTypes.OnPlayerBeginMove, Disable);
            EventManager.Subscribe(EventTypes.OnPlayerEndMove, Enable);
            EventManager.Subscribe(EventTypes.OnStartEnemyTurn, Disable);
            EventManager.Subscribe(EventTypes.OnEndEnemyTurn, Enable);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnActiveAllyChanged, OnActiveAllyChanged);
            EventManager.Unsubscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
            EventManager.Unsubscribe(EventTypes.OnPlayerBeginMove, Disable);
            EventManager.Unsubscribe(EventTypes.OnPlayerEndMove, Enable);
            EventManager.Unsubscribe(EventTypes.OnStartEnemyTurn, Disable);
            EventManager.Unsubscribe(EventTypes.OnEndEnemyTurn, Enable);
        }

        private void Update()
        {
            if (_allyTransform == null) return;
            
            _margin = (int)(Screen.width * marginFraction);
            
            Vector3 pos = _camera.WorldToScreenPoint(_allyTransform.position + allyOffset) + _offset;
            Vector3 clampedPos = new Vector3(
                Mathf.Clamp(pos.x, _margin, Screen.width - _margin),
                Mathf.Clamp(pos.y, _margin, Screen.height - _margin),
                0f
            );

            bool isOffScreen = !Mathf.Approximately(pos.x, clampedPos.x) || !Mathf.Approximately(pos.y, clampedPos.y);

            if (isOffScreen != _wasOffScreen)
            {
                _image.sprite = isOffScreen ? offScreenSprite : onScreenSprite;
                _wasOffScreen = isOffScreen;
            }

            float bob = isOffScreen
                ? 0f
                : Mathf.Sin(bobSpeed * Time.time) * bobMagnitude;

            transform.position = clampedPos + Vector3.up * bob;
        }

        private void OnActiveAllyChanged(object data)
        {
            if (data is not Ally ally)
            {
                Debug.LogWarning("Passed a non-ally to ActiveAllyIndicator::OnActiveAllyChanged");
                return;
            }

            _allyTransform = ally.transform;
        }

        private void OnCameraModeChanged(object data)
        {
            if (data is not CameraMode mode)
            {
                Debug.LogWarning("Passed a non-mode to ActiveAllyIndicator::OnCameraModeChanged");
                return;
            }

            _offset = mode == CameraMode.Standard ? standardOffset : airSupportOffset;
        }

        private void Enable()
        {
            _image.DOFade(1f, FadeTime);
        }

        private void Disable()
        {
            _image.DOFade(0f, FadeTime);
        }
    }
}
