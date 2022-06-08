using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;
using HarmonyLib;


namespace ACME
{
    /// <summary>
    /// Harmony patch to replace CameraControler.LateUpdate for FPS mode.
    /// </summary>
    [HarmonyPatch]
    public static class FPSPatch
    {
        // Limits.
        internal const float MinFPSKeySpeed = 0.2f;
        internal const float MaxFPSKeySpeed = 4.0f;


        // Control keys - movement.
        public static SavedInputKey cameraMoveLeft, cameraMoveRight, cameraMoveForward, cameraMoveBackward;

        // Control keys - rotation.
        public static SavedInputKey cameraRotateLeft, cameraRotateRight, cameraRotateUp, cameraRotateDown, cameraMouseRotate;

        // Delegates to private methods.
        public delegate void UpdateFreeCameraDelegate(CameraController __instance);
        private static UpdateFreeCameraDelegate updateFreeCam = Delegate.CreateDelegate(typeof(UpdateFreeCameraDelegate), typeof(CameraController).GetMethod("UpdateFreeCamera", BindingFlags.Instance | BindingFlags.NonPublic)) as UpdateFreeCameraDelegate;

        // Speed settings.
        private static float keyTurnSpeed = 1.0f, keyMoveSpeed = 1.0f, mouseTurnSpeed = 1.0f;


        /// <summary>
        // FPS turn speed.
        /// </summary>
        internal static float KeyTurnSpeed
        {
            get => keyTurnSpeed;

            set => keyTurnSpeed = Mathf.Clamp(value, MinFPSKeySpeed, MaxFPSKeySpeed);
        }


        /// <summary>
        // FPS move speed.
        /// </summary>
        internal static float KeyMoveSpeed
        {
            get => keyMoveSpeed;

            set => keyMoveSpeed = Mathf.Clamp(value, MinFPSKeySpeed, MaxFPSKeySpeed);
        }


        /// <summary>
        // FPS move speed.
        /// </summary>
        internal static float MouseTurnSpeed
        {
            get => mouseTurnSpeed;

            set => mouseTurnSpeed = Mathf.Clamp(value, MinFPSKeySpeed, MaxFPSKeySpeed);
        }


        /// <summary>
        /// Pre-emptive Harmony prefix for CameraController.LateUpdate to implement free FPS mod.
        /// Applied and unapplied manually.
        /// </summary>
        /// <param name="__instance">CameraController instance reference</param>
        /// <returns>Always false (never execute original method)</returns>
        public static bool LateUpdate(CameraController __instance)
        {
            // Set starting values.
            Vector2 cameraRotation = __instance.m_targetAngle;
            Vector3 cameraPosition = __instance.m_targetPosition;

            updateFreeCam.Invoke(__instance);

           // updateFreeCam();

            // Movement vector.
            Vector3 direction = Vector3.zero;

            // Ignore key input if UI has focus.
            if (!UIView.HasModalInput() && !Singleton<ToolManager>.instance.m_properties.HasInputFocus)
            {
                // Rotation keys,.
                float rotationSpeed = 60f * keyTurnSpeed;
                if (cameraRotateLeft.IsPressed())
                {
                    cameraRotation.x -= rotationSpeed * Time.deltaTime;
                }
                if (cameraRotateRight.IsPressed())
                {
                    cameraRotation.x += rotationSpeed * Time.deltaTime;
                }
                if (cameraRotateUp.IsPressed())
                {
                    cameraRotation.y -= rotationSpeed * Time.deltaTime;
                }
                if (cameraRotateDown.IsPressed())
                {
                    cameraRotation.y += rotationSpeed * Time.deltaTime;
                }

                // Movement keys.
                if (Input.GetKey(cameraMoveForward.Key))
                {
                    direction += Vector3.forward * keyMoveSpeed;
                }
                if (Input.GetKey(cameraMoveBackward.Key))
                {
                    direction += Vector3.back * keyMoveSpeed;
                }
                if (Input.GetKey(cameraMoveLeft.Key))
                {
                    direction += Vector3.left * keyMoveSpeed;
                }
                if (Input.GetKey(cameraMoveRight.Key))
                {
                    direction += Vector3.right * keyMoveSpeed;
                }
            }

            // Ignore mouse input if mouse is inside UI.
            if (!Singleton<ToolManager>.instance.m_properties.IsInsideUI)
            {
                // Mouse wheel.
                direction += Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 10f * keyMoveSpeed;

                // Mouse rotation.
                if (cameraMouseRotate.IsPressed() || SteamController.GetDigitalAction(SteamController.DigitalInput.RotateMouse))
                {
                    float speed = mouseTurnSpeed * 2f;
                    cameraRotation.x += Input.GetAxis("Mouse X") * speed;
                    cameraRotation.y -= Input.GetAxis("Mouse Y") * speed;
                }
            }

            // Input modifier.
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                direction *= 10f;
            }
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                direction /= 10f;
            }

            // Clamp cameraRotation.
            if (cameraRotation.x < -180f)
            {
                cameraRotation.x += 360f;
            }
            if (cameraRotation.x > 180f)
            {
                cameraRotation.x -= 360f;
            }

            // Caclulate new rotation and position (moving relative to current facing).
            Quaternion quaternion = Quaternion.AngleAxis(cameraRotation.x, Vector3.up) * Quaternion.AngleAxis(cameraRotation.y, Vector3.right);
            cameraPosition += quaternion * direction * 50f * Time.deltaTime;
            __instance.m_targetPosition = cameraPosition;
            __instance.m_targetAngle = cameraRotation;
            UpdateCurrentPosition(__instance);


            // Set controller fields to our updated values.
            __instance.transform.rotation = quaternion;
            __instance.transform.position = __instance.m_currentPosition;

            // Pre-empt original method.
            return false;
        }


        /// <summary>
        /// Harmony reverse patch to access private method CameraController.UpdateCurrentPosition.
        /// </summary>
		/// <param name="instance">CameraController instance</param>
        [HarmonyReversePatch]
        [HarmonyPatch((typeof(CameraController)), "UpdateCurrentPosition")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void UpdateCurrentPosition(CameraController instance)
        {
            Logging.Error("UpdateFreeCamera reverse Harmony patch wasn't applied, params: ", instance);
            throw new NotImplementedException("Harmony reverse patch not applied");
        }
    }
}