using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;
using World;

namespace Managers
{
    public class AirSupportManager : MonoBehaviour
    {
        [SerializeField] private AirSupportHUD airSupportHUD;
        [SerializeField] private List<ActionScriptableObject> availableActions;
        
        public Actions Actions;
        
        private Camera _camera;
        private LayerMask _cellLayer;
        public static AirSupportManager Instance;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
            
            Actions = new Actions(availableActions);
            _camera = Camera.main;
            _cellLayer = LayerMask.GetMask("Cell");
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnEndEnemyTurn, RefreshActions);
        }

        public Cell GetHoveredCell()
        {
            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
            if (Physics.Raycast(ray, out var hit, 100f, _cellLayer))
            {
                Vector2Int coords = new Vector2Int(
                    Mathf.RoundToInt(hit.transform.position.x),
                    Mathf.RoundToInt(hit.transform.position.z)
                    );
                
                return TacticsGrid.Instance.GetCell(coords);
            }

            return null;
        }

        private void RefreshActions()
        {
            Debug.Log("refresh");
            Actions.Refresh();
        }
    }
}