using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.Serialization;      // for IGameSerializable
using Entities;

[Serializable]
public class EquipmentCarrier : MonoBehaviour, IGameSerializable
{
    private static EquipmentCarrier _instance;
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

    private readonly Dictionary<string, Dictionary<EquipmentType, EquipmentScriptableObject>>
        _equipment = new Dictionary<string, Dictionary<EquipmentType, EquipmentScriptableObject>>();

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

    public void SetSoldierEquipment(string soldierName, EquipmentType slot, EquipmentScriptableObject equipment)
    {
        if (!_equipment.ContainsKey(soldierName))
            _equipment[soldierName] = new Dictionary<EquipmentType, EquipmentScriptableObject>();

        _equipment[soldierName][slot] = equipment;
    }

    public EquipmentScriptableObject GetSoldierEquipment(string soldierName, EquipmentType slot)
    {
        if (_equipment.TryGetValue(soldierName, out var slots) &&
            slots.TryGetValue(slot, out var so))
        {
            return so;
        }
        return null;
    }


    public bool Validate()
    {
        return true;
    }

    public string Serialize()
    {
        var entries = new List<EquipmentEntry>();
        foreach (var soldierKvp in _equipment)
        {
            foreach (var slotKvp in soldierKvp.Value)
            {
                entries.Add(new EquipmentEntry
                {
                    soldierName    = soldierKvp.Key,
                    slot           = slotKvp.Key,
                    resourceName   = slotKvp.Value.name
                });
            }
        }

        var wrapper = new EquipmentWrapper { entries = entries.ToArray() };
        return JsonUtility.ToJson(wrapper, true);
    }

    public void Deserialize(string json)
    {
        _equipment.Clear();

        var wrapper = JsonUtility.FromJson<EquipmentWrapper>(json);
        foreach (var e in wrapper.entries)
        {
            string folder = e.slot == EquipmentType.Ability ? "Ability" : "Equipment";
            var path = $"{folder}/{e.resourceName}";
            var so   = Resources.Load<EquipmentScriptableObject>(path);

            if (so != null)
            {
                SetSoldierEquipment(e.soldierName, e.slot, so);
                Debug.Log($"[EquipmentCarrier] Loaded SO: '{path}' for {e.soldierName} ({e.slot})");
            }
            else
            {
                Debug.LogError($"[EquipmentCarrier] Missing SO at Resources/{path}.asset");
            }

        }
    }

    [Serializable]
    private struct EquipmentEntry
    {
        public string          soldierName;
        public EquipmentType   slot;
        public string          resourceName;
    }

    [Serializable]
    private class EquipmentWrapper
    {
        public EquipmentEntry[] entries;
    }
}
