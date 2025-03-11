using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using World;

namespace Entities
{
    public class Ally : Entity
    {
        private MoveArea _moveArea;
        private HashSet<Cell> _reachableCells;
        [SerializeField] private string _name;
        private Abilities abilities;
        
        protected override void Awake()
        {
            base.Awake();

            _moveArea= GetComponentInChildren<MoveArea>();
        }

        protected override void Start()
        {
            base.Start();
            
            ApplyEquipmentModifiers();

            EventManager.TriggerEvent(EventTypes.OnSpawnAlly, this);
        }

        protected void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnActiveAllyChanged, OnActiveAllyChanged);
            EventManager.Subscribe(EventTypes.OnEndEnemyTurn, OnEndEnemyTurn);
        }
        
        protected void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnActiveAllyChanged, OnActiveAllyChanged);
            EventManager.Unsubscribe(EventTypes.OnEndEnemyTurn, OnEndEnemyTurn);

        }

        public override void TryMoveToCell(Cell destination)
        {
            if (!Actions.CanUseAction(ActionType.Move) || !_reachableCells.Contains(destination))
            {
                // Handle unable
                return;
            }
            
            EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.Move);
            _moveArea.Hide();
            Actions.UseAction(ActionType.Move);
            StartCoroutine(MoveToCell(destination));
        }

        protected override IEnumerator FollowPath(List<Cell> path)
        {
            EventManager.TriggerEvent(EventTypes.OnPlayerBeginMove, this);
            yield return base.FollowPath(path);
            EventManager.TriggerEvent(EventTypes.OnPlayerEndMove);
        }

        public void EnableThrow(int range)
        {
            var _possibleCells = Pathfinder.FindReachableCells(CurrentCell, range);
            _moveArea.GenerateMesh(_possibleCells, CurrentCell.Position);
            _moveArea.Show();
        }

        public void DisableThrow()
        {
            _reachableCells = Pathfinder.FindReachableCells(CurrentCell, Data.MovementRange);
            _moveArea.GenerateMesh(_reachableCells, CurrentCell.Position);
        }

        public void TryThrow(ThrowableScriptableObject throwable, Cell destination, HashSet<Cell> area)
        {
            if (!Actions.CanUseAction(ActionType.Ability) || !_reachableCells.Contains(destination))
            {
                // Handle unable
                return;
            }
            
            EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.Ability);
            _moveArea.Hide();
            Actions.UseAction(ActionType.Ability);
            
            //TODO: Probably going to want a parabolic throw arc here

            AbilityFunction ability;
            bool allies;

            switch (throwable.ability)
            {
                //Enemy-focused abilities
                case AbilityType.Frag:
                    ability = abilities.DamageAbility; 
                    allies = false;
                    break;
                case AbilityType.EMP:
                    ability = abilities.DisarmAbility;
                    allies = false;
                    break;
                //Ally-focused abilities
                case AbilityType.Care: 
                    ability = abilities.HealAbility; 
                    allies = true;
                    break;

                default:
                    ability = abilities.DebugAbility;
                    allies = false;
                    break;
            }

            abilities.DoForAllInArea(ability, allies, area);
        }

        private void OnActiveAllyChanged(object data)
        {
            if (data is not Ally ally) return;
            if (ally == this)
            {
                if (Actions.CanUseAction(ActionType.Move))
                {
                    _moveArea.Show();
                }
            }
            else
            {
                _moveArea.Hide();
            }
            
            _reachableCells = Pathfinder.FindReachableCells(CurrentCell, Data.MovementRange);
            _moveArea.GenerateMesh(_reachableCells, CurrentCell.Position);
        }

        private void OnEndEnemyTurn()
        {
            Actions.Refresh();
        }

        public void SetMoveMeshActive(bool toggle)
        {
            if (toggle) _moveArea.Show();
            else _moveArea.Hide();
        }

        private void ApplyEquipmentModifiers()
        {
            EquipmentScriptableObject armor = EquipmentCarrier.Instance.GetSoldierEquipment(_name, EquipmentType.Armor);
            EquipmentScriptableObject boots = EquipmentCarrier.Instance.GetSoldierEquipment(_name, EquipmentType.Boots);

            if (armor == null || boots == null) 
            {
                Debug.LogWarning("ERROR - Equipment is NULL");
                Modifiers = new UnitModifiers(); 
                return; 
            }

            int percentDamageReduction = armor.modifiers.PercentDamageReduction + boots.modifiers.PercentDamageReduction;
            int percentDamageReturnChance = armor.modifiers.PercentDamageReturnChance + boots.modifiers.PercentDamageReturnChance;
            int percentDamageReturnAmount = armor.modifiers.PercentDamageReturnAmount + boots.modifiers.PercentDamageReturnAmount;
            int evasionBonusPercent = armor.modifiers.EvasionBonusPercent + boots.modifiers.EvasionBonusPercent;
            int bonusMovementRange = armor.modifiers.BonusMovementRange + boots.modifiers.BonusMovementRange;
            int abilityCooldownTurnReduction = armor.modifiers.AbilityCooldownTurnReduction + boots.modifiers.AbilityCooldownTurnReduction;
            
            Modifiers = new UnitModifiers
            {
                PercentDamageReduction = percentDamageReduction,
                PercentDamageReturnChance = percentDamageReturnChance,
                PercentDamageReturnAmount = percentDamageReturnAmount,
                EvasionBonusPercent = evasionBonusPercent,
                BonusMovementRange = bonusMovementRange,
                AbilityCooldownTurnReduction = abilityCooldownTurnReduction
            };
        }
    }
}