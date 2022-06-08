using UnityEngine;


namespace ACME
{
    internal static class MapDragging
    {
        // Bounds.
        internal const float MinDragSpeed = 0.2f;
        internal const float MaxDragSpeed = 2.0f;


        // Map dragging settings;
        private static float dragXdirection = 1.0f, dragYdirection = 1.0f, dragSpeed = 1.0f;

        // Map dragging status.
        private static bool isDragging = false;


        /// <summary>
        /// Map dragging X axis inversion.
        /// </summary>
        internal static bool InvertXDrag
        {
            get => dragXdirection < 0f;

            set => dragXdirection = value ? -1.0f : 1.0f;
        }


        /// <summary>
        /// Map dragging Y axis inversion.
        /// </summary>
        internal static bool InvertYDrag
        {
            get => dragYdirection < 0f;

            set => dragYdirection = value ? -1.0f : 1.0f;
        }


        /// <summary>
        // Map dragging speed.
        /// </summary>
        internal static float DragSpeed
        {
            get => dragSpeed;

            set => dragSpeed = Mathf.Clamp(value, MinDragSpeed, MaxDragSpeed);
        }


        /// <summary>
        /// Implements mouse map dragging.
        /// </summary>
        /// <param name="controller">Cameraa controller reference</param>
        public static void MapDrag(CameraController controller)
        {
            // Is the rotate button pressed?
            if (ModSettings.mapDragKey.IsPressed())
            {
                // Get screen mouse movement direction.
                Vector3 direction = new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y"));

                // Speed multiplier based on camera size and user setting.
                float heightSpeedFactor = controller.m_currentSize / 30f;
                if (heightSpeedFactor < 1f)
                {
                    // Minimum 1.5.
                    heightSpeedFactor = 1f;
                }
                float speedMultiplier = heightSpeedFactor * dragSpeed;

                // Apply speed multiplier and direction factors.
                direction.x *= speedMultiplier * dragXdirection;
                direction.z *= speedMultiplier * dragYdirection;

                // Convert screen movement to relative to camera rotation.
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, controller.transform.right);
                controller.m_targetPosition += quaternion * direction;

                // Lock cursor while dragging and set active flag height.
                Cursor.lockState = CursorLockMode.Locked;
                isDragging = true;
            }
            else if (isDragging)
            {
                // We're dragging but the button is now released - unlock cursor and clear status.
                Cursor.lockState = CursorLockMode.None;
                isDragging = false;
            }
        }
    }
}