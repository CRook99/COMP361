using System.Collections.Generic;
using UnityEngine;
using Entities;
using UnityEngine.UI;
using TMPro; 

public class SoldierWidget : MonoBehaviour
{
    [SerializeField] private string SoldierName;
    public string soldierName => SoldierName;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private Image SoldierImage;
    [SerializeField] private EquipmentWidget ArmorWidget;
    [SerializeField] private EquipmentWidget BootsWidget;
    // [SerializeField] private AbilityWidget AbilityWidget

    private Dictionary<EquipmentType, EquipmentScriptableObject> _selectedEquipment = new();

    private void Start()
    {
        NameText.text = SoldierName;
        if (ArmorWidget != null) 
        {
            ArmorWidget.Initialize(this, EquipmentType.Armor);
        }

        if (BootsWidget != null) 
        {
            BootsWidget.Initialize(this, EquipmentType.Boots);
        }
    }

    public EquipmentScriptableObject GetEquipment(EquipmentType type)
    {
        return type == EquipmentType.Armor ? ArmorWidget.GetSelectedEquipment() :
               type == EquipmentType.Boots ? BootsWidget.GetSelectedEquipment() : null;
    }
}