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
            EventManager.Subscribe(EventTypes.OnStartAllyTurn, OnStartTurn);
            EventManager.Subscribe(EventTypes.OnActiveAllyChanged, OnActiveAllyChanged);
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
            StartCoroutine(MoveToCell(destination));
        }

        protected override IEnumerator FollowPath(List<Cell> path)
        {
            EventManager.TriggerEvent(EventTypes.OnPlayerBeginMove, this);
            yield return base.FollowPath(path);
            EventManager.TriggerEvent(EventTypes.OnPlayerEndMove);
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
        }

        private void OnStartTurn()
        {
            _reachableCells = Pathfinder.FindReachableCells(CurrentCell, Data.MovementRange);
            _moveArea.GenerateMesh(_reachableCells, CurrentCell.Position);
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