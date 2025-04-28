using System;

namespace Entities.DTOs
{
    [Serializable]
    public class DropCoverDTO
    {
        public SerializableVector2Int[] positions;
        public int dropCoverCooldown;

    }

}
