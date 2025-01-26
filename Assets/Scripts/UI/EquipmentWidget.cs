using System;
using System.Collections;
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

    private void Awake()
    {
        LeftButton.onClick.AddListener(CycleLeft);
        RightButton.onClick.AddListener(CycleRight);
        
        RefreshWidget();
    }

    private void CycleLeft()
    {
        // We use a custom mod since C# mod is actually remainder
        _itemIndex = MathUtils.Mod(_itemIndex - 1, EquipmentItems.Count);
        RefreshWidget();
    }

    private void CycleRight()
    {
        // We use a custom mod since C# mod is actually remainder
        _itemIndex = MathUtils.Mod(_itemIndex + 1, EquipmentItems.Count);
        RefreshWidget();
    }

    private void RefreshWidget()
    {
        EquipmentScriptableObject newEquipment = EquipmentItems[_itemIndex];
        NameText.text = newEquipment.title;
        DescriptionText.text = newEquipment.description;
        EquipmentIcon.sprite = newEquipment.image;
    }
}
