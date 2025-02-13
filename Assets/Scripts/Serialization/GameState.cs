using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.Serialization;  // Assuming IGameSerializable and GameSerializableBase are here

[Serializable]
public class GameState : GameSerializableBase
{
    public List<AllyData> Allies;
    public List<EnemyData> Enemies;

    public override bool Validate()
    {
        // Add further checks here in future
        return Allies != null && Enemies != null;
    }
}

