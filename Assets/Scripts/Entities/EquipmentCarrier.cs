using System.Collections.Generic;
using UnityEngine;
using Entities;
using System;

/*
    * EquipmentCarrier is a class that is used to hold equipment of Alies through scenes.
*/
public class EquipmentCarrier : MonoBehaviour
{
    private static EquipmentCarrier _instance;
    private Dictionary<string, Dictionary<EquipmentType, EquipmentScriptableObject>> _equipment = new();

    public static EquipmentCarrier Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("EquipmentCarrier");
                _instance = go.AddComponent<EquipmentCarrier>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSoldierEquipment(string soldierName, EquipmentType equipmentType, EquipmentScriptableObject equipment)
    {
        if (!_equipment.ContainsKey(soldierName))
        {
            _equipment[soldierName] = new Dictionary<EquipmentType, EquipmentScriptableObject>();
        }
        _equipment[soldierName][equipmentType] = equipment;
    }


    public EquipmentScriptableObject GetSoldierEquipment(string soldierName, EquipmentType equipmentType)
    {
        if (_equipment.ContainsKey(soldierName))
        {
            if (_equipment[soldierName].ContainsKey(equipmentType))
            {
                return _equipment[soldierName][equipmentType];
            }
        }
        return null;
    }
}