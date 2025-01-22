using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "Entity", menuName = "ScriptableObjects/Entity")]
    public class EntityScriptableObject : ScriptableObject
    {
        public string Name;
        public int MaxHealth;
        public int MovementRange;
    }
}