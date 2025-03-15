using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
    public class AbilityScriptableObject : EquipmentScriptableObject
    {
        public int Cooldown;
        public AbilityType ability;
    }
}
