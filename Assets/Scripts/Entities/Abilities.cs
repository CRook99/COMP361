using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities{

    public enum AbilityType
    {
        Frag,
        EMP,
        Care
    }

    public delegate void AbilityFunction(Entity entity);

    public class Abilities
    {
        private readonly int FRAG_DAMAGE = 20;
        private readonly int CARE_HEAL = 15;
        private readonly int EMP_DISARM = 1;

        public void HealAbility(Entity entity)
        {
            if (entity is not Ally ally) return;
            ally.Heal(CARE_HEAL);
        }

        public void DamageAbility(Entity entity)
        {
            if (entity is not Enemy enemy) return;
            enemy.TakeDamage(FRAG_DAMAGE);
        }

        public void DisarmAbility(Entity entity)
        {
            if (entity is not Enemy enemy) return;
            //TODO: Implementation
        }

        public void DebugAbility(Entity entity)
        {
            Debug.Log(entity + "got hit with a big ouchie");
        }

        public void DoForAllInArea(AbilityFunction f, bool allies, HashSet<Cell> area)
        {
            if (allies)
            {
                foreach (Ally ally in GameManager.Allies)
                {
                    if (area.Contains(ally.CurrentCell)) f(ally);
                }
            }
            else 
            {
                foreach (Enemy enemy in GameManager.Enemies)
                {
                    if (area.Contains(enemy.CurrentCell)) f(enemy);
                }
            }
        }
    }
}
