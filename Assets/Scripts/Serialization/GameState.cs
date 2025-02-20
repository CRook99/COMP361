using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.Serialization;  // Assuming IGameSerializable and GameSerializableBase are here

[Serializable]
public class GameState : GameSerializableBase
{
    public List<AllyData> Allies;
    public List<EnemyData> Enemies;
    public bool isAllyTurn;
    public int turnNumber;

    // Statistics
    public int enemiesVanquished;
    public int damageDealt;
    public int shotsLanded;
    public int spacesMoved;
    public int fallenSoldiers;
    public int damageReceived;
    public int shotsTaken;
    public int chanceShotsDodged;

    public override bool Validate()
    {
        // Add further checks here in future
        return Allies != null && Enemies != null && turnNumber > 0;
    }
}

