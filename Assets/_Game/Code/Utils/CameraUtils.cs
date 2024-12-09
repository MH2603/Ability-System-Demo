using UnityEngine;

namespace Utils
{
    public static class CameraUtils
    {
        public static Vector3 ProjectPositionTowardCamera(this Camera camera, Vector3 fromPosition, float moveTowardCameraAmount, out float scale)
        {
            // Project the position of the card toward the camera,
            // then scale the card so it looks the same as it did before.
            Vector3 cameraPosition = camera.transform.position;
            float zDistanceToCamera = fromPosition.z - cameraPosition.z;

            if (zDistanceToCamera != 0.0f)
            {
                float percentageToMoveTowardCamera = moveTowardCameraAmount / zDistanceToCamera;
                scale = 1.0f - percentageToMoveTowardCamera;
                return Vector3.LerpUnclamped(fromPosition, cameraPosition, percentageToMoveTowardCamera);
            }
            else
            {
                scale = 1.0f;
                return fromPosition;
            }
        }
    }
}
