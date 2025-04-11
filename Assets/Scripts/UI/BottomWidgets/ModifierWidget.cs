using System;
using UnityEngine;
using UnityEngine.UI;
using Entities;
using Managers;
using TMPro;

public class ModifierWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI modifierText;
    
    private void OnEnable()
    {
        EventManager.Subscribe(EventTypes.OnActiveAllyChanged, OnActiveAllyChanged);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(EventTypes.OnActiveAllyChanged, OnActiveAllyChanged);
    }

    private void OnActiveAllyChanged(object data)
    {
        if (data is Entity unit)
        {
            UpdateModifierText(unit.Modifiers);
        }
    }

    private void UpdateModifierText(UnitModifiers modifiers)
    {
        modifierText.text = ""; // Clear existing text

        if (modifiers.PercentDamageReduction > 0)
            modifierText.text += $"Damage Reduction: {modifiers.PercentDamageReduction}%\n";

        if (modifiers.PercentDamageReturnChance > 0)
            modifierText.text += $"Return Chance: {modifiers.PercentDamageReturnChance}%\n";

        if (modifiers.PercentDamageReturnAmount > 0)
            modifierText.text += $"Return Amount: {modifiers.PercentDamageReturnAmount}%\n";

        if (modifiers.EvasionBonusPercent > 0)
            modifierText.text += $"Evasion Bonus: {modifiers.EvasionBonusPercent}%\n";

        if (modifiers.PercentBonusWeaponDamage > 0)
            modifierText.text += $"Damage Bonus: {modifiers.PercentBonusWeaponDamage}%\n";

        if (modifiers.BonusMovementRange > 0)
            modifierText.text += $"Bonus Movement: {modifiers.BonusMovementRange} tiles\n";

        //if (modifiers.AbilityCooldownTurnReduction > 0)
            //modifierText.text += $"Cooldown Reduction: {modifiers.AbilityCooldownTurnReduction} turns\n";
    }
}
