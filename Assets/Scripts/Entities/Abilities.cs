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

    public static class Abilities
    {
        private static readonly int FRAG_DAMAGE = 20;
        private static readonly int CARE_HEAL = 15;
        private static readonly int EMP_DISARM = 1;

        public static void HealAbility(Entity entity)
        {
            if (entity is not Ally ally) return;
            ally.Heal(CARE_HEAL);
        }

        public static void DamageAbility(Entity entity)
        {
            if (entity is not Enemy enemy) return;
            enemy.TakeDamage(FRAG_DAMAGE);
        }

        public static void DisarmAbility(Entity entity)
        {
            if (entity is not Enemy enemy) return;
            enemy.Disarm(EMP_DISARM);
        }

        public static void DebugAbility(Entity entity)
        {
            Debug.Log("DEBUG");
        }

        public static void GetAbilityInfoFromEnum(AbilityType type, out AbilityFunction func, out bool allies)
        {
            switch (type)
            {
                //Enemy-focused abilities
                case AbilityType.Frag:
                    func = DamageAbility;
                    allies = false;
                    break;
                case AbilityType.EMP:
                    func = DisarmAbility;
                    allies = false;
                    break;
                //Ally-focused abilities
                case AbilityType.Care: 
                    func = HealAbility; 
                    allies = true;
                    break;

                default:
                    func = DebugAbility;
                    allies = false;
                    break;
            }
        }

        public static void DoForAllInArea(AbilityFunction f, bool allies, HashSet<Cell> area)
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
