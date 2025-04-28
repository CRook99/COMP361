using System;
using UnityEngine;

namespace Entities.DTOs
{
    [Serializable]
    public struct SerializableVector2Int
    {
        public int x;
        public int y;

        public SerializableVector2Int(Vector2Int v)
        {
            x = v.x;
            y = v.y;
        }

        public Vector2Int ToVector2Int() => new Vector2Int(x, y);
    }
}
