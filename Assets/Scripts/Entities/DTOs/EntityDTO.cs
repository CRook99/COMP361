// EntityDTO.cs
using System;
using UnityEngine;
using Entities;

[Serializable]
public class EntityDTO {
    public int uniqueId; // New field for the unique identifier

    public float posX;
    public float posY;
    public float posZ;
    public int health;
    public string entityDataName;

    public UnitModifiers modifiers;
}
