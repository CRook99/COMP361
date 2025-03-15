using Entities;

namespace System
{
    public struct ShotData
    {
        public Entity Shooter;
        public Entity Target;
        public CoverTypes Cover;
        public int Damage;
    }
}