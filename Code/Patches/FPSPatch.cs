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

        // Coontrol keys - absolute movement.
        public static KeybindingKey absUp = new KeybindingKey(KeyCode.PageUp);
        public static KeybindingKey absDown = new KeybindingKey(KeyCode.PageDown);
        public static KeybindingKey absLeft = new KeybindingKey(KeyCode.LeftArrow);
        public static KeybindingKey absRight = new KeybindingKey(KeyCode.RightArrow);
        public static KeybindingKey absForward = new KeybindingKey(KeyCode.UpArrow);
        public static KeybindingKey absBack = new KeybindingKey(KeyCode.DownArrow);

        // Control keys - movement.
        public static KeybindingKey cameraMoveForward = new KeybindingKey(KeyCode.W);
        public static KeybindingKey cameraMoveBackward = new KeybindingKey(KeyCode.S);
        public static KeybindingKey cameraMoveLeft = new KeybindingKey(KeyCode.A);
        public static KeybindingKey cameraMoveRight = new KeybindingKey(KeyCode.D);

        // Control keys - rotation.
        public static KeybindingKey cameraRotateLeft = new KeybindingKey(KeyCode.Q);
        public static KeybindingKey cameraRotateRight = new KeybindingKey(KeyCode.E);
        public static KeybindingKey cameraRotateUp = new KeybindingKey(KeyCode.R);
        public static KeybindingKey cameraRotateDown = new KeybindingKey(KeyCode.F);
        public static KeybindingKey cameraMouseRotate = new KeybindingKey(KeyCode.Mouse2);

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
            Vector3 absDirection = Vector3.zero;

            // Ignore key input if UI has focus.
            if (!UIView.HasModalInput() && !Singleton<ToolManager>.instance.m_properties.HasInputFocus)
            {
                // Rotation keys.
                float rotationSpeed = 60f * keyTurnSpeed;
                if (Input.GetKey(cameraRotateLeft.key))
                {
                    cameraRotation.x -= rotationSpeed * Time.deltaTime;
                }
                if (Input.GetKey(cameraRotateRight.key))
                {
                    cameraRotation.x += rotationSpeed * Time.deltaTime;
                }
                if (Input.GetKey(cameraRotateUp.key))
                {
                    cameraRotation.y -= rotationSpeed * Time.deltaTime;
                }
                if (Input.GetKey(cameraRotateDown.key))
                {
                    cameraRotation.y += rotationSpeed * Time.deltaTime;
                }

                // Movement keys - relative.
                bool altDown = Input.GetKey(KeyCode.LeftAlt) | Input.GetKey(KeyCode.RightAlt) | Input.GetKey(KeyCode.AltGr);
                if (Input.GetKey(cameraMoveForward.key))
                {
                    direction += Vector3.forward * keyMoveSpeed;
                }
                if (Input.GetKey(cameraMoveBackward.key))
                {
                    direction += Vector3.back * keyMoveSpeed;
                }
                if (Input.GetKey(cameraMoveLeft.key))
                {
                    direction += Vector3.left * keyMoveSpeed;
                }
                if (Input.GetKey(cameraMoveRight.key))
                {
                    direction += Vector3.right * keyMoveSpeed;
                }

                // Movement keys - absolute.
                if (Input.GetKey(absForward.key))
                {
                    absDirection += Vector3.forward * keyMoveSpeed;
                }
                if (Input.GetKey(absBack.key))
                {
                    absDirection += Vector3.back * keyMoveSpeed;
                }
                if (Input.GetKey(absLeft.key))
                {
                    absDirection += Vector3.left * keyMoveSpeed;
                }
                if (Input.GetKey(absRight.key))
                {
                    absDirection += Vector3.right * keyMoveSpeed;
                }
                if (Input.GetKey(absUp.key))
                {
                    absDirection += Vector3.up * keyMoveSpeed;
                }
                if (Input.GetKey(absDown.key))
                {
                    absDirection += Vector3.down * keyMoveSpeed;
                }
            }


            // Ignore mouse input if mouse is inside UI.
            if (!Singleton<ToolManager>.instance.m_properties.IsInsideUI)
            {
                // Mouse wheel.
                direction += Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 10f * keyMoveSpeed;

                // Mouse rotation.
                if (Input.GetKey(cameraMouseRotate.key) || SteamController.GetDigitalAction(SteamController.DigitalInput.RotateMouse))
                {
                    float speed = mouseTurnSpeed * 2f;
                    cameraRotation.x += Input.GetAxis("Mouse X") * speed;
                    cameraRotation.y -= Input.GetAxis("Mouse Y") * speed;
                }
            }

            // Input modifier.
            float distance = Time.deltaTime * 50f;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                distance *= 10f;
            }
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                distance /= 10f;
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

            // Calculate new rotation and position (moving relative to current facing).
            Quaternion xQuad = Quaternion.AngleAxis(cameraRotation.x, Vector3.up);
            Quaternion xyQuad = xQuad * Quaternion.AngleAxis(cameraRotation.y, Vector3.right);
            cameraPosition += xyQuad * direction * distance;

            // Absolute movement.
            cameraPosition += xQuad * absDirection * distance;

            // Apply new position and rotation.
            __instance.m_targetPosition = cameraPosition;
            __instance.m_targetAngle = cameraRotation;
            UpdateCurrentPosition(__instance);

            // Set controller fields to our updated values.
            __instance.transform.rotation = xyQuad;
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