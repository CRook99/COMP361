using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace UI.BottomWidgets
{
    public enum EBottomWidget
    {
        Base,
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
        private Stack<BottomWidget> _activeWidgets;
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
            _activeWidgets = new Stack<BottomWidget>();
            
            foreach (Transform tr in transform)
            {
                if (!tr.TryGetComponent(out BottomWidget widget)) return;
                
                _widgets.Add(widget.Type, widget);
            }
            
            Show(EBottomWidget.Base);
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnCameraModeChanged, OnCameraModeChanged);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Show(EBottomWidget.Weapon);
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                Show(EBottomWidget.Ability);
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                HideActive();
            }
        }

        public void Show(EBottomWidget type)
        {
            if (!_widgets.ContainsKey(type)) return;

            BottomWidget newWidget = _widgets[type];
            if (newWidget is SubBottomWidget newSubWidget)
            {
                if (_activeWidgets.Peek() is SubBottomWidget) // Can hide active widget as it is sub
                {
                    HideActive();
                }

                newSubWidget.OnClickBackButton += HideActive;
            }
            else // New widget is base
            {
                HideAllActive();
            }
            
            newWidget.Show();
            _activeWidgets.Push(newWidget);
        }

        public void HideActive()
        {
            if (_activeWidgets.Count == 0 || _activeWidgets.Peek() is not SubBottomWidget) return;

            SubBottomWidget popWidget = _activeWidgets.Pop() as SubBottomWidget;
            popWidget!.Hide();
            popWidget.OnClickBackButton -= HideActive;
        }

        public void HideAllActive()
        {
            while (_activeWidgets.Count > 0)
            {
                _activeWidgets.Pop().Hide();
            }
        }

        private void OnCameraModeChanged(object data)
        {
            if (data is not CameraMode mode) return;
            
            Show(mode == CameraMode.Standard ? EBottomWidget.Base : EBottomWidget.AirSupportBase);
        }
    }
}