using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UI.BottomWidgets;
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
        public AbilityScriptableObject ChosenAbility;
        public AbilityScriptableObject DELETEME;

        public HashSet<Cell> ReachableCells => _reachableCells;
        
        protected override void Awake()
        {
            base.Awake();

            _moveArea= GetComponentInChildren<MoveArea>();
            abilities = new Abilities();
        }

        protected override void Start()
        {
            base.Start();
            
            LoadEquipment();

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
            if (!Actions.CanUseAction(ActionType.Move))
            {
                HintManager.Instance.Hint("Already moved with this soldier!", HintLevel.Error);
                return;
            }

            if (!_reachableCells.Contains(destination))
            {
                HintManager.Instance.Hint("Cannot reach this tile!", HintLevel.Error);
                return;
            }
            
            EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.Move);
            _moveArea.Hide();
            Actions.UseAction(ActionType.Move);
            HintManager.Instance.FulfilTutorial(TutorialSteps.Movement);
            StartCoroutine(MoveToCell(destination));
        }

        protected override IEnumerator FollowPath(List<Cell> path)
        {
            EventManager.TriggerEvent(EventTypes.OnPlayerBeginMove, this);
            yield return base.FollowPath(path);
            EventManager.TriggerEvent(EventTypes.OnPlayerEndMove);
        }

        public void EnableThrow(int range, out HashSet<Cell> cells)
        {
            _reachableCells = Pathfinder.FindReachableCells(CurrentCell, range, true);
            cells = _reachableCells;
            _moveArea.GenerateMesh(_reachableCells, CurrentCell.Position);
            _moveArea.Show();
        }

        public void DisableThrow()
        {
            _reachableCells = Pathfinder.FindReachableCells(CurrentCell, GetMovementRange(), true);
            _moveArea.GenerateMesh(_reachableCells, CurrentCell.Position);
        }

        public void TryThrow(ThrowableScriptableObject throwable, Cell destination, HashSet<Cell> area)
        {
            if (!Actions.CanUseAction(ActionType.Ability))
            {
                HintManager.Instance.Hint("You can't use that right now.", HintLevel.Error);
                return;
            }

            if (!_reachableCells.Contains(destination))
            {
                HintManager.Instance.Hint("Target cell is out of range", HintLevel.Error);
                return;
            }
            
            EventManager.TriggerEvent(EventTypes.OnPlayerUseAction, ActionType.Ability);
            _moveArea.Hide();
            Actions.UseAction(ActionType.Ability);
            BottomWidgetManager.Instance.Show(EBottomWidget.Movement);
            
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
            EventManager.TriggerEvent(EventTypes.OnPlayerEndAbility);
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
            
            _reachableCells = Pathfinder.FindReachableCells(CurrentCell, GetMovementRange(), true);
            _moveArea.GenerateMesh(_reachableCells, CurrentCell.Position);
        }

        private void OnEndEnemyTurn()
        {
            Actions.Tick();
        }

        public void SetMoveMeshActive(bool toggle)
        {
            if (toggle && Actions.CanUseAction(ActionType.Move)) _moveArea.Show();
            else _moveArea.Hide();
        }
        
        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);
      
            EventManager.TriggerEvent(EventTypes.OnDamageTaken, amount); // stats manager
        }

        private void LoadEquipment()
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
            //int abilityCooldownTurnReduction = armor.modifiers.AbilityCooldownTurnReduction + boots.modifiers.AbilityCooldownTurnReduction;
            
            Modifiers = new UnitModifiers
            {
                PercentDamageReduction = percentDamageReduction,
                PercentDamageReturnChance = percentDamageReturnChance,
                PercentDamageReturnAmount = percentDamageReturnAmount,
                EvasionBonusPercent = evasionBonusPercent,
                BonusMovementRange = bonusMovementRange,
                //AbilityCooldownTurnReduction = abilityCooldownTurnReduction
            };

            AbilityScriptableObject ability = EquipmentCarrier.Instance.GetSoldierEquipment(_name, EquipmentType.Ability) as AbilityScriptableObject;
            if (ability != null)
            {
                ChosenAbility = ability;
            }
        }
    }
}