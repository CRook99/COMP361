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
    [SerializeField] private EquipmentWidget AbilityWidget;

    private void Start()
    {
        NameText.text = SoldierName;
        if (ArmorWidget != null) 
        {
            ArmorWidget.Initialize(this, EquipmentType.Armor);
        } else 
        {
            Debug.Log("ArmorWidget is null");
        }

        if (BootsWidget != null) 
        {
            BootsWidget.Initialize(this, EquipmentType.Boots);
        } else 
        {
            Debug.Log("BootsWidget is null");
        }

        if (AbilityWidget != null) 
        {
            AbilityWidget.Initialize(this, EquipmentType.Ability);
        } else 
        {
            Debug.Log("AbilityWidget is null");
        }
    }

    public EquipmentScriptableObject GetEquipment(EquipmentType type)
    {
        return type switch
        {
            EquipmentType.Armor => ArmorWidget.GetSelectedEquipment(),
            EquipmentType.Boots => BootsWidget.GetSelectedEquipment(),
            EquipmentType.Ability => AbilityWidget.GetSelectedEquipment(),
            _ => null
        };
    }
}