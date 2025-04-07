using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using Controller;
using Managers;
using UI;
using UnityEngine;
using Utility;
using World;
using Utility.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3; 

namespace Entities
{
    public abstract class Entity : MonoBehaviour, IGameSerializable
    {
        private static int nextAllyUniqueId = 0;
        private static int nextEnemyUniqueId = 0;
        private static int nextUniqueId = 0;

        [SerializeField] private int uniqueId = -1; // default value
        public int UniqueId => uniqueId;

        /** Speed that I move along my movement path. For movement range, Data.MovementRange */
        private const float MovementSpeed = 4f;

        public EntityScriptableObject Data;
        
        public UnitModifiers Modifiers;
        public Actions Actions;
        public Transform GeometryTransform;

        public event Action<int> OnHealthChanged;

        public Cell CurrentCell => TacticsGrid.Instance.GetCell((int)transform.position.x, (int)transform.position.z);
        public int CurrentHealth;

        [SerializeField] private Transform centerOfMass;

        public Transform CenterOfMass
        {
            get => centerOfMass != null ? centerOfMass : transform; // Fallback to transform if null
            protected set => centerOfMass = value;
        }

        protected virtual void Awake()
        {
            if (uniqueId < 0)
            {
                if (this is Ally)
                {
                    uniqueId = nextAllyUniqueId;
                    nextAllyUniqueId++;
                }
                else if (this is Enemy)
                {
                    uniqueId = nextEnemyUniqueId;
                    nextEnemyUniqueId++;
                }
                else
                {
                    // Fallback in case there are other types
                    uniqueId = nextUniqueId;
                    nextUniqueId++;
                }
            }

            CurrentHealth = Data.MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth); // Force widget refresh
            
            Actions = new Actions(Data.AvailableActions);
        }

        protected virtual void Start()
        {
            CurrentCell.Walkable = false;
        }

        [ContextMenu("Test Range")]
        private void TestRange()
        {
            HashSet<Cell> cells = Pathfinder.FindReachableCells(CurrentCell, 5, true);
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
            if (destination == CurrentCell) yield break;
            List<Cell> path = Pathfinder.FindPath(CurrentCell, destination);
            if (path.Count == 0) yield break;

            EventManager.TriggerEvent(EventTypes.OnSpaceMoved, path.Count); // stats manager
            
            Debug.Log($"Moving to {destination.Position}");

            Actions.UseAction(ActionType.Move);
            
            CurrentCell.Walkable = true;
            destination.Walkable = false;
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

                Vector3 direction = end - start;
                direction.y = 0f;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(direction);
                    GeometryTransform.rotation = lookRot;
                }

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

        // Helper function to get the cell at or adjacent to the current cell,
        // where it would have a line of sight to the entity, it also returns the distance to the entity
        // returns null if not possible to find a line of sight
        public (Cell, float distance) FindPeakableCellInLightOfSight(Entity entity) {
            if(!TacticsGrid.Instance.ObstacleBetweenCells(CurrentCell, entity.CurrentCell)) {
                Vector3 cellPosition3D = new Vector3(CurrentCell.Position.x, 0, CurrentCell.Position.y);
                float distance = Vector3.Distance(cellPosition3D, entity.transform.position);
                return (CurrentCell, distance);
            }

            List<Cell> neighbourCells = new List<Cell>
            {
                CurrentCell.N, CurrentCell.S, CurrentCell.E, CurrentCell.W
            };
            
            foreach (Cell cell in neighbourCells)
            {
                if (cell == null || !cell.Walkable) continue;
                if (TacticsGrid.Instance.ObstacleBetweenCells(cell, entity.CurrentCell))
                    continue;
                
                Vector3 cellPosition3D = new Vector3(cell.Position.x, 0, cell.Position.y);
                float distance = Vector3.Distance(cellPosition3D, entity.transform.position);

                return (cell, distance);
            }

            // Not possible to get line of sight
            return (null, -1);
        }

        public void TakeDamage(int amount)
        {
            if (amount == 0) return;
            
            CurrentHealth -= amount;
            CurrentHealth = Mathf.Max(CurrentHealth, 0);
            OnHealthChanged?.Invoke(CurrentHealth);
            
            UIManager.Instance.DamageNumbers.Animate(transform.position, amount);

            EventManager.TriggerEvent(EventTypes.OnDamageTaken, amount); // stats manager

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            CurrentHealth += amount;
            CurrentHealth = Mathf.Min(CurrentHealth, Data.MaxHealth); // Clamp health to maximum
            OnHealthChanged?.Invoke(CurrentHealth);
        }

        protected virtual void Die()
        {
            Debug.Log($"{gameObject.name} died!");

            EventManager.TriggerEvent(EventTypes.OnAllyFallen); // stats manager
        }

        // A public that function that checks if an obstacle exists between the current cell
        // of the enemy and the input cell
        public bool HasObstacleBetween(Cell end) {
            return TacticsGrid.Instance.ObstacleBetweenCells(CurrentCell, end);
        }

        // --- IGameSerializable Implementation ---
        public virtual bool Validate() {
            return CurrentHealth >= 0;
        }

        public virtual string Serialize() {
            EntityDTO data = new EntityDTO {
                uniqueId = uniqueId, 
                posX = transform.position.x,
                posY = transform.position.y,
                posZ = transform.position.z,
                health = CurrentHealth,
                entityDataName = Data != null ? Data.name : "Unknown",
                modifiers = Modifiers
            };
            return JsonUtility.ToJson(data, true);
        }

        public virtual void Deserialize(string json) {
            EntityDTO data = JsonUtility.FromJson<EntityDTO>(json);
            transform.position = new Vector3(data.posX, data.posY, data.posZ);
            CurrentHealth = data.health;
            Modifiers = data.modifiers;
        }
    }
}