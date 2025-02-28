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
        
        public EntityScriptableObject Data;
        
        public UnitModifiers Modifiers;
        public EntityActions Actions;

        public event Action<int> OnHealthChanged;
        public event Action<int> OnTakeDamage;

        public Cell CurrentCell => TacticsGrid.Instance.GetCell((int)transform.position.x, (int)transform.position.z);
        protected int CurrentHealth;


        protected virtual void Awake()
        {
            CurrentHealth = Data.MaxHealth;
            Actions = new EntityActions();
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            
        }

        [ContextMenu("Test Range")]
        private void TestRange()
        {
            HashSet<Cell> cells = Pathfinder.FindReachableCells(CurrentCell, 5);
            foreach (Cell cell in cells)
            {
                Debug.Log(cell.Position);
            }
        }

        protected void Initialize(EntityScriptableObject inData)
        {
            if (inData == null)
                Debug.LogWarning($"Unit provided with null data.");

            Data = inData;
        }

        public abstract void TryMoveToCell(Cell destination);

        public IEnumerator MoveToCell(Cell destination)
        {
            List<Cell> path = Pathfinder.FindPath(CurrentCell, destination);
            if (path.Count == 0) yield break;

            EventManager.TriggerEvent(EventTypes.OnSpaceMoved, path.Count); // stats manager
            
            Debug.Log($"Moving to {destination.Position}");

            Actions.UseAction(ActionType.Move);
            yield return FollowPath(path);
        }

        protected virtual IEnumerator FollowPath(List<Cell> path)
        {
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
            
        }

        protected void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            OnTakeDamage?.Invoke(amount);
            OnHealthChanged?.Invoke(CurrentHealth);

            EventManager.TriggerEvent(EventTypes.OnDamageTaken, amount); // stats manager

            if (CurrentHealth <= 0)
            {
                Die();
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

             EventManager.TriggerEvent(EventTypes.OnAllyFallen); // stats manager
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

        // A public that function that checks if an obstacle exists between the current cell
        // of the enemy and the input cell
        public bool HasObstacleBetween(Cell end) {
            return TacticsGrid.Instance.ObstacleBetweenCells(CurrentCell, end);
        }
    }
}