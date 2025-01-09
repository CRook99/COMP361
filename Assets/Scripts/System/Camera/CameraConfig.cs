using UnityEngine;

namespace System.Camera
{
    [Serializable]
    public class CameraConfig
    {
        public Vector3 Offset;
        
        [Header("Movement")] 
        public float MovementSpeed;

        [Header("Rotation")] 
        public float RotationTime;

        [Header("Zoom")] 
        public float ZoomSpeed;
        public float MinZoomDistance;
        public float MaxZoomDistance;
    }
}