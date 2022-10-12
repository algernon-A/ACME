// <copyright file="FPSPatch.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using AlgernonCommons;
    using AlgernonCommons.Keybinding;
    using ColossalFramework;
    using ColossalFramework.UI;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmony patch to replace CameraControler.LateUpdate for FPS mode.
    /// </summary>
    [HarmonyPatch]
    public static class FPSPatch
    {
        /// <summary>
        /// Minimum FPS key speed.
        /// </summary>
        internal const float MinFPSKeySpeed = 0.2f;

        /// <summary>
        /// Maximum FPS key speed.
        /// </summary>
        internal const float MaxFPSKeySpeed = 4.0f;

        private static readonly UpdateFreeCameraDelegate UpdateFreeCam = Delegate.CreateDelegate(typeof(UpdateFreeCameraDelegate), typeof(CameraController).GetMethod("UpdateFreeCamera", BindingFlags.Instance | BindingFlags.NonPublic)) as UpdateFreeCameraDelegate;

        // Speed settings.
        private static float s_keyTurnSpeed = 1.0f;
        private static float s_keyMoveSpeed = 1.0f;
        private static float s_mouseTurnSpeed = 1.0f;

        // Control keys - absolute movement.
        private static KeyOnlyBinding s_absUp = new KeyOnlyBinding(KeyCode.PageUp);
        private static KeyOnlyBinding s_absDown = new KeyOnlyBinding(KeyCode.PageDown);
        private static KeyOnlyBinding s_absLeft = new KeyOnlyBinding(KeyCode.LeftArrow);
        private static KeyOnlyBinding s_absRight = new KeyOnlyBinding(KeyCode.RightArrow);
        private static KeyOnlyBinding s_absForward = new KeyOnlyBinding(KeyCode.UpArrow);
        private static KeyOnlyBinding s_absBack = new KeyOnlyBinding(KeyCode.DownArrow);

        // Control keys - movement.
        private static KeyOnlyBinding s_cameraMoveForward = new KeyOnlyBinding(KeyCode.W);
        private static KeyOnlyBinding s_cameraMoveBackward = new KeyOnlyBinding(KeyCode.S);
        private static KeyOnlyBinding s_cameraMoveLeft = new KeyOnlyBinding(KeyCode.A);
        private static KeyOnlyBinding s_cameraMoveRight = new KeyOnlyBinding(KeyCode.D);

        // Control keys - rotation.
        private static KeyOnlyBinding s_cameraRotateLeft = new KeyOnlyBinding(KeyCode.Q);
        private static KeyOnlyBinding s_cameraRotateRight = new KeyOnlyBinding(KeyCode.E);
        private static KeyOnlyBinding s_cameraRotateUp = new KeyOnlyBinding(KeyCode.R);
        private static KeyOnlyBinding s_cameraRotateDown = new KeyOnlyBinding(KeyCode.F);
        private static KeyOnlyBinding s_cameraMouseRotate = new KeyOnlyBinding(KeyCode.Mouse2);

        // Delegate to private method.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
        private delegate void UpdateFreeCameraDelegate(CameraController __instance);

        /// <summary>
        /// Gets or sets the control key for absolute movement up.
        /// </summary>
        internal static KeyOnlyBinding AbsUp { get => s_absUp; set => s_absUp = value; }

        /// <summary>
        /// Gets or sets the control key for absolute movement down.
        /// </summary>
        internal static KeyOnlyBinding AbsDown { get => s_absDown; set => s_absDown = value; }

        /// <summary>
        /// Gets or sets the control key for absolute movement left.
        /// </summary>
        internal static KeyOnlyBinding AbsLeft { get => s_absLeft; set => s_absLeft = value; }

        /// <summary>
        /// Gets or sets the control key for absolute movement right.
        /// </summary>
        internal static KeyOnlyBinding AbsRight { get => s_absRight; set => s_absRight = value; }

        /// <summary>
        /// Gets or sets the control key for- absolute movement forward.
        /// </summary>
        internal static KeyOnlyBinding AbsForward { get => s_absForward; set => s_absForward = value; }

        /// <summary>
        /// Gets or sets the control key for absolute movement backwards.
        /// </summary>
        internal static KeyOnlyBinding AbsBack { get => s_absBack; set => s_absBack = value; }

        /// <summary>
        /// Gets or sets the control key for relative movement forward.
        /// </summary>
        internal static KeyOnlyBinding CameraMoveForward { get => s_cameraMoveForward; set => s_cameraMoveForward = value; }

        /// <summary>
        /// Gets or sets the control key for relative movement backwards.
        /// </summary>
        internal static KeyOnlyBinding CameraMoveBackward { get => s_cameraMoveBackward; set => s_cameraMoveBackward = value; }

        /// <summary>
        /// Gets or sets the control key for relative movement left.
        /// </summary>
        internal static KeyOnlyBinding CameraMoveLeft { get => s_cameraMoveLeft; set => s_cameraMoveLeft = value; }

        /// <summary>
        /// Gets or sets the control key for relative movement right.
        /// </summary>
        internal static KeyOnlyBinding CameraMoveRight { get => s_cameraMoveRight; set => s_cameraMoveRight = value; }

        /// <summary>
        /// Gets or sets the control key for rotate left.
        /// </summary>
        internal static KeyOnlyBinding CameraRotateLeft { get => s_cameraRotateLeft; set => s_cameraRotateLeft = value; }

        /// <summary>
        /// Gets or sets the control key for rotate right.
        /// </summary>
        internal static KeyOnlyBinding CameraRotateRight { get => s_cameraRotateRight; set => s_cameraRotateRight = value; }

        /// <summary>
        /// Gets or sets the control key for rotate up.
        /// </summary>
        internal static KeyOnlyBinding CameraRotateUp { get => s_cameraRotateUp; set => s_cameraRotateUp = value; }

        /// <summary>
        /// Gets or sets the control key for rotate down.
        /// </summary>
        internal static KeyOnlyBinding CameraRotateDown { get => s_cameraRotateDown; set => s_cameraRotateDown = value; }

        /// <summary>
        /// Gets or sets the control key for enable mouse look.
        /// </summary>
        internal static KeyOnlyBinding CameraMouseRotate { get => s_cameraMouseRotate; set => s_cameraMouseRotate = value; }

        /// <summary>
        /// Gets or sets FPS turn speed.
        /// </summary>
        internal static float KeyTurnSpeed
        {
            get => s_keyTurnSpeed;

            set => s_keyTurnSpeed = Mathf.Clamp(value, MinFPSKeySpeed, MaxFPSKeySpeed);
        }

        /// <summary>
        /// Gets or sets FPS move speed.
        /// </summary>
        internal static float KeyMoveSpeed
        {
            get => s_keyMoveSpeed;

            set => s_keyMoveSpeed = Mathf.Clamp(value, MinFPSKeySpeed, MaxFPSKeySpeed);
        }

        /// <summary>
        /// Gets or sets FPS mouse turnng speed.
        /// </summary>
        internal static float MouseTurnSpeed
        {
            get => s_mouseTurnSpeed;

            set => s_mouseTurnSpeed = Mathf.Clamp(value, MinFPSKeySpeed, MaxFPSKeySpeed);
        }

        /// <summary>
        /// Pre-emptive Harmony prefix for CameraController.LateUpdate to implement free FPS mod.
        /// Applied and unapplied manually.
        /// </summary>
        /// <param name="__instance">CameraController instance reference.</param>
        /// <returns>Always false (never execute original method).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
        public static bool LateUpdate(CameraController __instance)
        {
            // Set starting values.
            Vector2 cameraRotation = __instance.m_targetAngle;
            Vector3 cameraPosition = __instance.m_targetPosition;

            UpdateFreeCam.Invoke(__instance);

            // updateFreeCam();

            // Movement vector.
            Vector3 direction = Vector3.zero;
            Vector3 absDirection = Vector3.zero;

            // Ignore key input if UI has focus.
            if (!UIView.HasModalInput() && !Singleton<ToolManager>.instance.m_properties.HasInputFocus)
            {
                // Rotation keys.
                float rotationSpeed = 60f * s_keyTurnSpeed;
                if (s_cameraRotateLeft.IsPressed())
                {
                    cameraRotation.x -= rotationSpeed * Time.deltaTime;
                }

                if (s_cameraRotateRight.IsPressed())
                {
                    cameraRotation.x += rotationSpeed * Time.deltaTime;
                }

                if (s_cameraRotateUp.IsPressed())
                {
                    cameraRotation.y -= rotationSpeed * Time.deltaTime;
                }

                if (s_cameraRotateDown.IsPressed())
                {
                    cameraRotation.y += rotationSpeed * Time.deltaTime;
                }

                // Movement keys - relative.
                if (s_cameraMoveForward.IsPressed())
                {
                    direction += Vector3.forward * s_keyMoveSpeed;
                }

                if (s_cameraMoveBackward.IsPressed())
                {
                    direction += Vector3.back * s_keyMoveSpeed;
                }

                if (s_cameraMoveLeft.IsPressed())
                {
                    direction += Vector3.left * s_keyMoveSpeed;
                }

                if (s_cameraMoveRight.IsPressed())
                {
                    direction += Vector3.right * s_keyMoveSpeed;
                }

                // Movement keys - absolute.
                if (s_absForward.IsPressed())
                {
                    absDirection += Vector3.forward * s_keyMoveSpeed;
                }

                if (s_absBack.IsPressed())
                {
                    absDirection += Vector3.back * s_keyMoveSpeed;
                }

                if (s_absLeft.IsPressed())
                {
                    absDirection += Vector3.left * s_keyMoveSpeed;
                }

                if (s_absRight.IsPressed())
                {
                    absDirection += Vector3.right * s_keyMoveSpeed;
                }

                if (s_absUp.IsPressed())
                {
                    absDirection += Vector3.up * s_keyMoveSpeed;
                }

                if (s_absDown.IsPressed())
                {
                    absDirection += Vector3.down * s_keyMoveSpeed;
                }
            }

            // Ignore mouse input if mouse is inside UI.
            if (!Singleton<ToolManager>.instance.m_properties.IsInsideUI)
            {
                // Mouse wheel.
                direction += Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 10f * s_keyMoveSpeed;

                // Mouse rotation.
                if (s_cameraMouseRotate.IsPressed() || SteamController.GetDigitalAction(SteamController.DigitalInput.RotateMouse))
                {
                    float speed = s_mouseTurnSpeed * 2f;
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
        /// <param name="instance">CameraController instance.</param>
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(CameraController), "UpdateCurrentPosition")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void UpdateCurrentPosition(CameraController instance)
        {
            Logging.Error("UpdateFreeCamera reverse Harmony patch wasn't applied, params: ", instance);
            throw new NotImplementedException("Harmony reverse patch not applied");
        }
    }
}