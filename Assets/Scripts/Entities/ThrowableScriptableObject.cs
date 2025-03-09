using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "Throwable", menuName = "ScriptableObjects/Throwable")]
    public class ThrowableScriptableObject : AbilityScriptableObject
    {
        public int ThrowRadius;
        public int EffectRadius;
    }
}
