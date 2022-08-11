// <copyright file="FPSMode.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System;
    using System.Reflection;
    using AlgernonCommons;
    using UnityEngine;

    /// <summary>
    ///  Class to manage free FPS camera mode.
    /// </summary>
    internal static class FPSMode
    {
        // Status flag.
        private static bool s_modeActive = false;

        /// <summary>
        /// Toggles FPS mode.
        /// </summary>
        internal static void ToggleMode()
        {
            s_modeActive = !s_modeActive;

            CameraController controller = CameraUtils.Controller;

            // Get assigned controls if we're activating.
            if (s_modeActive)
            {
                // Adjust target position to match current position.
                controller.ClearTarget();
                controller.m_targetPosition = controller.transform.position;
                controller.m_currentPosition = controller.transform.position;
            }
            else
            {
                // Deactivating mode; set CameraController position to match current position.
                // Reverse game CameraController.UpdateTransform calculations.
                float verticalOffset = controller.m_currentSize * Mathf.Max(0f, 1f - (controller.m_currentHeight / controller.m_maxDistance)) / Mathf.Tan((float)Math.PI / 180f * CameraUtils.MainCamera.fieldOfView);
                Quaternion quaternion = Quaternion.AngleAxis(controller.m_currentAngle.x, Vector3.up) * Quaternion.AngleAxis(controller.m_currentAngle.y, Vector3.right);
                Vector3 cameraPos = controller.m_currentPosition + (quaternion * new Vector3(0f, 0f, 0f - verticalOffset));
                cameraPos.y += CameraController.CalculateCameraHeightOffset(cameraPos, verticalOffset);
                cameraPos = CameraController.ClampCameraPosition(cameraPos);
                cameraPos += controller.m_cameraShake * Mathf.Sqrt(verticalOffset);

                // Adjust controller position to account for the above.
                Vector3 adjustedPosition = controller.m_targetPosition + controller.transform.position - cameraPos;
                controller.m_targetPosition = adjustedPosition;
                controller.m_currentPosition = adjustedPosition;
                controller.m_targetHeight = adjustedPosition.y;
                controller.m_currentHeight = adjustedPosition.y;

                // Also update m_cachedPosition.
                FieldInfo m_cachedPosition = typeof(CameraController).GetField("m_cachedPosition", BindingFlags.Instance | BindingFlags.NonPublic);
                if (m_cachedPosition == null)
                {
                    Logging.Error("m_cachedPosition is null");
                }
                else
                {
                    m_cachedPosition.SetValue(controller, adjustedPosition);
                }
            }

            // Apply/remove FPS patch.
            Patcher.Instance.PatchFPS(s_modeActive);

            // Log message.
            Logging.Message("FPS mode ", s_modeActive ? "enabled" : "disabled");
        }
    }
}