using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Entities;
using Unity.VisualScripting;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Managers
{
    public struct ThrowInfo
    {
        public ThrowableScriptableObject ScriptableObject;
        public Cell Origin;
        public Cell Target;
        public HashSet<Cell> Area;

        public ThrowInfo(ThrowableScriptableObject so, Cell origin, Cell target, HashSet<Cell> area)
        {
            ScriptableObject = so;
            Origin = origin;
            Target = target;
            Area = area;
        }
    }
    
    public class ThrowManager : PlayerComponent
    {
        public float ThrowTime;
        public float ThrowHeight;

        public static ThrowManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        public void HandleThrow(ThrowInfo info)
        {
            StartCoroutine(ThrowSequence(info));
        }

        private IEnumerator ThrowSequence(ThrowInfo info)
        {
            InputManager.Instance.PlayerInput.Disable();
            CameraController.MoveToPosition(info.Target.Position.ToVector3XZ());

            yield return new WaitForSeconds(0.5f); // Wait for camera move

            Transform projectileTransform = Instantiate(info.ScriptableObject.ProjectileModel, info.Origin.Position.ToVector3XZ(1f), Quaternion.identity).transform;
            Vector3 rotation = Random.onUnitSphere;
            
            float elapsed = 0f;
            while (elapsed < ThrowTime)
            {
                float t = elapsed / ThrowTime;
                Vector3 start = info.Origin.Position.ToVector3XZ();
                Vector3 end = info.Target.Position.ToVector3XZ();
                Vector3 mid = Vector3.Lerp(start, end, t);
                float parabola = 4 * ThrowHeight * (t - t * t);
                mid.y = Mathf.Lerp(start.y, end.y, t) + parabola;

                projectileTransform.transform.position = mid;
                projectileTransform.transform.Rotate(rotation * 1f);

                elapsed += Time.deltaTime;
                yield return null;
            }
            
            ParticleSystem ps = Instantiate(info.ScriptableObject.ParticleSystem, projectileTransform.transform.position, Quaternion.identity);
            ps.Play();
            
            Abilities.GetAbilityInfoFromEnum(info.ScriptableObject.ability, out AbilityFunction ability, out bool allies);
            Abilities.DoForAllInArea(ability, allies, info.Area);
            
            Destroy(projectileTransform.gameObject);
            
            EventManager.TriggerEvent(EventTypes.OnPlayerEndAbility);
            InputManager.Instance.PlayerInput.Enable();
        }
    }
}