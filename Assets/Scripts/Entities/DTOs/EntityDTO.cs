// EntityDTO.cs
using System;
using Entities;

[Serializable]
public class EntityDTO {
    public int    uniqueId;         // matches Entity.UniqueId
    public float  posX, posY, posZ;
    public int    health;
    public string entityDataName;
    public UnitModifiers modifiers;

    [Serializable]
    public struct ActionCooldownEntry {
        public ActionType actionType;
        public int        currentCooldown;
    }

    public ActionCooldownEntry[] actionCooldowns;
}
