using System.Collections.Generic;
using Config;
using Entities;
using UI;
using UnityEngine;

namespace Managers
{
    public class StatusWidget : MonoBehaviour
    {
        [SerializeField] private AllyStatusWidget statusWidgetPrefab;
        [SerializeField] private Transform widgetContainer;
        [SerializeField] private List<ActionScriptableObject> actions;

        private Dictionary<Ally, AllyStatusWidget> _widgetMap;

        private void Awake()
        {
            _widgetMap = new();
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnSpawnAlly, RegisterAlly);
        }
    
        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnSpawnAlly, RegisterAlly);
        }

        public void RegisterAlly(object data)
        {
            if (data is not Ally ally) return;
        
            AllyStatusWidget widget = Instantiate(statusWidgetPrefab, widgetContainer);
            widget.Initialize(ally, actions);
            _widgetMap[ally] = widget;
        }
    }
}
