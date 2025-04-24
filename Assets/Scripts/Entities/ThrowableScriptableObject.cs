using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "Throwable", menuName = "ScriptableObjects/Throwable")]
    public class ThrowableScriptableObject : AbilityScriptableObject
    {
        public int ThrowRadius;
        public int EffectRadius;

        public GameObject ProjectileModel;
        public ParticleSystem ParticleSystem;
    }
}
