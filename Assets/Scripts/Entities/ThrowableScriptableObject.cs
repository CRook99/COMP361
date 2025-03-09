using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "Throwable", menuName = "ScriptableObjects/Throwable")]
    public class ThrowableScriptableObject : EquipmentScriptableObject
    {
        public int ThrowRadius;
        public int EffectRadius;
        public int Cooldown;
        public AbilityType ability;
    }
}
