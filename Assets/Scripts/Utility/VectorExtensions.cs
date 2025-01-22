using UnityEngine;

namespace Utility
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3XZ(this Vector2Int v, float y = 0f)
        {
            return new Vector3(v.x, y, v.y);
        }
    }
}