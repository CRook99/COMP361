using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "Throwable", menuName = "ScriptableObjects/Throwable")]
    public class ThrowableScriptableObject : ScriptableObject
    {
        public EquipmentScriptableObject equipment;
        public int ThrowRadius;
        public int EffectRadius;
        public int Cooldown;
        public AbilityType ability;
    }
}
