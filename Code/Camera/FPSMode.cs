using System;
using System.Reflection;
using UnityEngine;
using ColossalFramework;


namespace ACME
{
    /// <summary>
    ///  Class to manage free FPS camera mode.
    /// </summary>
    internal static class FPSMode
    {
        internal static bool modeActive = false;
        internal static void ToggleMode()
        {
            modeActive = !modeActive;

            CameraController controller = CameraUtils.Controller;

            // Get assigned controls if we're activating.
            if (modeActive)
            {
                // Get game keybinding fields.
                FieldInfo m_cameraMouseRotate = typeof(CameraController).GetField("m_cameraMouseRotate", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo m_cameraRoteateUp = typeof(CameraController).GetField("m_cameraRotateUp", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo m_cameraRotateDown = typeof(CameraController).GetField("m_cameraRotateDown", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo m_cameraRotateLeft = typeof(CameraController).GetField("m_cameraRotateLeft", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo m_cameraRotateRight = typeof(CameraController).GetField("m_cameraRotateRight", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo m_cameraMoveLeft = typeof(CameraController).GetField("m_cameraMoveLeft", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo m_cameraMoveRight = typeof(CameraController).GetField("m_cameraMoveRight", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo m_cameraMoveForward = typeof(CameraController).GetField("m_cameraMoveForward", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo m_cameraMoveBackward = typeof(CameraController).GetField("m_cameraMoveBackward", BindingFlags.Instance | BindingFlags.NonPublic);

                // Assign game keybinding values to FPSpatch.
                FPSPatch.cameraMouseRotate = m_cameraMouseRotate.GetValue(controller) as SavedInputKey;
                FPSPatch.cameraRotateUp = m_cameraRoteateUp.GetValue(controller) as SavedInputKey;
                FPSPatch.cameraRotateDown = m_cameraRotateDown.GetValue(controller) as SavedInputKey;
                FPSPatch.cameraRotateLeft = m_cameraRotateLeft.GetValue(controller) as SavedInputKey;
                FPSPatch.cameraRotateRight = m_cameraRotateRight.GetValue(controller) as SavedInputKey;
                FPSPatch.cameraMoveLeft = m_cameraMoveLeft.GetValue(controller) as SavedInputKey;
                FPSPatch.cameraMoveRight = m_cameraMoveRight.GetValue(controller) as SavedInputKey;
                FPSPatch.cameraMoveForward = m_cameraMoveForward.GetValue(controller) as SavedInputKey;
                FPSPatch.cameraMoveBackward = m_cameraMoveBackward.GetValue(controller) as SavedInputKey;

                // Adjust target position to match current position.
                controller.ClearTarget();
                controller.m_targetPosition = controller.transform.position;
                controller.m_currentPosition = controller.transform.position;
            }
            else
            {
                // Deactivating mode; set CameraController position to match current position.
                // Reverse game CameraController.UpdateTransform calculations.
                float verticalOffset = controller.m_currentSize * Mathf.Max(0f, 1f - controller.m_currentHeight / controller.m_maxDistance) / Mathf.Tan((float)Math.PI / 180f * CameraUtils.MainCamera.fieldOfView);
                Quaternion quaternion = Quaternion.AngleAxis(controller.m_currentAngle.x, Vector3.up) * Quaternion.AngleAxis(controller.m_currentAngle.y, Vector3.right);
                Vector3 cameraPos = controller.m_currentPosition + quaternion * new Vector3(0f, 0f, 0f - verticalOffset);
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
            Patcher.PatchFPS(modeActive);

            // Log message.
            Logging.Message("FPS mode ", modeActive ? "enabled" : "disabled");
        }
    }
}