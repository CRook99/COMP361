using Entities;
using UnityEngine;

namespace System
{
    public struct ShotData
    {
        public Entity Shooter;
        public Entity Target;
        public CoverTypes Cover;
        public GameObject CoverObject;
        public int TotalDamage;
        
        // Modifiers
        public int ReturnDamage;
    }
}