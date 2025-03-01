using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Equipment")]
    public class EquipmentScriptableObject : ScriptableObject
    {
        public string title;
        public EquipmentType type;
        
        [TextArea]
        public string description;
        public Sprite image;
        public UnitModifiers modifiers;
    }
}
