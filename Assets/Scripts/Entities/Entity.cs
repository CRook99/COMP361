using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Utility;
using World;

namespace Entities
{
    public abstract class Entity : MonoBehaviour
    {
        private const float MovementSpeed = 4f;
        
        protected Cell CurrentCell;

        [SerializeField] protected EntityScriptableObject Data;
        protected int CurrentHealth;

        protected void Awake()
        {
            CurrentHealth = Data.MaxHealth;
        }

        protected void Start()
        {
            CurrentCell = TacticsGrid.Instance.GetCell(11, 11);
            transform.position = CurrentCell.Position.ToVector3XZ();
        }

        private void Update()
        {
            
        }

        protected void MoveToCell(Cell destination)
        {
            List<Cell> path = Pathfinder.FindPath(CurrentCell, destination);
            if (path.Count <= 1) return;
            
            StartCoroutine(FollowPath(path));
        }

        private IEnumerator FollowPath(List<Cell> path)
        {
            int pathIndex = 0;

            while (true)
            {
                Vector3 start = path[pathIndex].Position.ToVector3XZ();
                Vector3 end = path[pathIndex + 1].Position.ToVector3XZ();
                Debug.Log(start);
                Debug.Log(end);
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
    }
}