using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace UI.BottomWidgets
{
    public enum EBottomWidget
    {
        Movement,
        Weapon,
        Ability,
        AirSupportBase,
        CoverDrop,
        Airstrike,
    }
    
    public class BottomWidgetManager : MonoBehaviour
    {
        public static BottomWidgetManager Instance {get; private set;}
        
        private Dictionary<EBottomWidget, BottomWidget> _widgets;
        private BottomWidget _activeWidget;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            
            _widgets = new Dictionary<EBottomWidget, BottomWidget>();
            
            foreach (Transform tr in transform)
            {
                if (!tr.TryGetComponent(out BottomWidget widget)) return;
                
                _widgets.Add(widget.Type, widget);
            }
        }

        private void Start()
        {
            Show(EBottomWidget.Movement);
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
        }

        public void Show(EBottomWidget type)
        {
            if (!_widgets.ContainsKey(type))
            {
                Debug.LogWarning($"BottomWidgetManager did not contain key {type}");
                return;
            }

            var newWidget = _widgets[type];
            if (!newWidget.CanOpen())
            {
                HintManager.Instance.Hint("Can't use this right now!", HintLevel.Normal);
                return;
            }
            
            if (_activeWidget != null) _activeWidget.Close();
            _activeWidget = _widgets[type];
            _activeWidget.Open();
        }

        private void OnCameraModeChanged(object data)
        {
            if (data is not CameraMode mode) return;
            
            Show(mode == CameraMode.Standard ? EBottomWidget.Movement : EBottomWidget.AirSupportBase);
        }
    }
}