using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Controller;
using Managers;
using UnityEngine;
using Utility;
using World;

namespace Entities
{
    public abstract class Entity : MonoBehaviour
    {
        /** Speed that I move along my movement path. For movement range, Data.MovementRange */
        private const float MovementSpeed = 4f;
        
        [SerializeField] protected EntityScriptableObject Data;
        
        public UnitModifiers Modifiers;

        protected Cell CurrentCell => TacticsGrid.Instance.GetCell((int)transform.position.x, (int)transform.position.z);
        protected int CurrentHealth;


        protected void Awake()
        {
            CurrentHealth = Data.MaxHealth;
        }

        protected void Start()
        {
            
        }

        private void Update()
        {
            
        }

        protected void Initialize(EntityScriptableObject inData)
        {
            if (inData == null)
                Debug.LogWarning($"Unit provided with null data.");

            Data = inData;
        }

        public void MoveToCell(Cell destination)
        {
            List<Cell> path = Pathfinder.FindPath(CurrentCell, destination);
            if (path.Count <= 1) return;
            
            StartCoroutine(FollowPath(path));
        }

        protected virtual IEnumerator FollowPath(List<Cell> path)
        {
            EventManager.TriggerEvent(EventTypes.OnPlayerBeginMove, this);
            
            int pathIndex = 0;

            while (true)
            {
                Vector3 start = path[pathIndex].Position.ToVector3XZ();
                Vector3 end = path[pathIndex + 1].Position.ToVector3XZ();
                float travelTime = Vector3.Distance(start, end) / MovementSpeed;
                float elapsed = 0f;

                while (elapsed < travelTime)
                {
                    transform.position = Vector3.Lerp(start, end, elapsed / travelTime);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                transform.position = end;
                pathIndex++;

                if (pathIndex >= path.Count - 1) break;
            }
            
            EventManager.TriggerEvent(EventTypes.OnPlayerEndMove);
        }

        protected void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth <= 0)
            {
                // Die
            }
        }

        protected void Heal(int amount)
        {
            CurrentHealth += amount;
            CurrentHealth = Mathf.Min(amount, Data.MaxHealth); // Clamp health to maximum
        }

        protected virtual void Die()
        {
            Debug.Log($"{gameObject.name} died!");
        }

        public virtual void TakeTurn(System.Action onTurnComplete)
        {
            Debug.Log($"{gameObject.name} is taking their turn.");
            StartCoroutine(EndTurnAfterDelay(onTurnComplete));
        }

        private IEnumerator EndTurnAfterDelay(System.Action onTurnComplete)
        {
            yield return new WaitForSeconds(1f); 
            onTurnComplete?.Invoke(); 
        }
    }
}