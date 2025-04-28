using System.Collections.Generic;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class EquipmentWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI DescriptionText;
    [SerializeField] private Image EquipmentIcon;
    [SerializeField] private List<EquipmentScriptableObject> EquipmentItems;

    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;

    private int _itemIndex;
    private SoldierWidget _parentSoldier;
    private EquipmentType _equipmentType;

    public void Initialize(SoldierWidget soldier, EquipmentType equipmentType)
    {
        _parentSoldier = soldier;
        _equipmentType = equipmentType;
        RefreshWidget();
    }

    private void Awake()
    {
        LeftButton.onClick.AddListener(() => CycleEquipment(-1));
        RightButton.onClick.AddListener(() => CycleEquipment(1));
        RefreshWidget();
    }

    private void CycleEquipment(int direction) 
    {
        _itemIndex = MathUtils.Mod(_itemIndex + direction, EquipmentItems.Count);
        RefreshWidget();
    }

    private void RefreshWidget()
    {
        EquipmentScriptableObject newEquipment = EquipmentItems[_itemIndex];
        NameText.text = newEquipment.title;
        DescriptionText.text = newEquipment.description;
        EquipmentIcon.sprite = newEquipment.image;
    }

    public EquipmentScriptableObject GetSelectedEquipment()
    {
        return EquipmentItems[_itemIndex];
    }
}
