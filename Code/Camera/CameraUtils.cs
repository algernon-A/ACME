// <copyright file="CameraUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System.Reflection;
    using AlgernonCommons;
    using UnityEngine;

    /// <summary>
    /// Utility class for changing camera settings.
    /// </summary>
    public static class CameraUtils
    {
        /// <summary>
        /// Minimum near clip distance.
        /// </summary>
        internal const float MinNearClip = 0.1f;

        /// <summary>
        /// Maximum near clip distance.
        /// </summary>
        internal const float MaxNearClip = 10f;

        /// <summary>
        /// Minimum camera distance from target.
        /// </summary>
        internal const float MinDistance = 2;

        /// <summary>
        /// Maximum camera distance from target.
        /// </summary>
        internal const float MaxDistance = 44000f;

        /// <summary>
        /// Minimum minimum shadow distance.
        /// </summary>
        internal const float MinMinShadowDistance = 20f;

        /// <summary>
        /// Maximum minimum shadow distance.
        /// </summary>
        internal const float MaxMinShadowDistance = 2500f;

        /// <summary>
        /// Minimum maximum shadow distance.
        /// </summary>
        internal const float MinMaxShadowDistance = 3000f;

        /// <summary>
        /// Maximum maximum shadow distance.
        /// </summary>
        internal const float MaxMaxShadowDistance = 14000f;

        // CameraManager m_originalNearPlane.
        private static readonly FieldInfo OriginalNearPlane = typeof(CameraController).GetField("m_originalNearPlane", BindingFlags.NonPublic | BindingFlags.Instance);

        // Current near clip plane distance (game default is 5).
        private static float s_nearClipPlane = 1f;

        // CameraController reference.
        private static CameraController s_cameraController;

        // Main camera reference.
        private static Camera s_mainCamera;

        // Shadow parameters.
        private static float s_minShadowDistance = 400f;
        private static float s_maxShadowDistance = 4000f;

        /// <summary>
        /// Sets the camera's initial saved position.
        /// </summary>
        internal static CameraPositions.SavedPosition InitialPosition { private get; set; } = default;

        /// <summary>
        /// Gets the CameraController reference.
        /// </summary>
        internal static CameraController Controller
        {
            get
            {
                // Get camera controller if we haven't already, and main camera has been instantiated.
                if (s_cameraController == null)
                {
                    s_cameraController = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<CameraController>();
                }

                return s_cameraController;
            }
        }

        /// <summary>
        /// Gets the main camera reference.
        /// </summary>
        internal static Camera MainCamera
        {
            get
            {
                // Get main camera if we haven't already.
                if (s_mainCamera == null)
                {
                    s_mainCamera = Controller.GetComponent<Camera>();
                }

                return s_mainCamera;
            }
        }

        /// <summary>
        /// Gets or sets the camera controllers base near clip plane base distance, in metres.
        /// Applied to the game's camera controller (not directly to the main camera), as that clobbers the camera's nearClipPlane every LateUpdate.
        /// </summary>
        internal static float NearClipPlane
        {
            get => s_nearClipPlane;

            set
            {
                // Clamp value to min and max permitted.
                s_nearClipPlane = Mathf.Clamp(value, MinNearClip, MaxNearClip);

                // Set field with updated value (only if game has loaded, e.g. Controller isn't null).
                CameraController controller = Controller;
                if (controller != null)
                {
                    OriginalNearPlane.SetValue(controller, s_nearClipPlane);
                }
            }
        }

        /// <summary>
        /// Gets or sets the camera controller's minimum shadow distance, which is used to determine shadow clarity.
        /// </summary>
        internal static float MinShadowDistance
        {
            get => s_minShadowDistance;

            set
            {
                // Clamp value to min and max permitted.
                s_minShadowDistance = Mathf.Clamp(value, MinMinShadowDistance, MaxMinShadowDistance);

                // Set field with updated value (only if game has loaded, e.g. Controller isn't null).
                CameraController controller = Controller;
                if (controller != null)
                {
                    controller.m_minShadowDistance = s_minShadowDistance;
                }
            }
        }

        /// <summary>
        /// Gets or sets the camera controller's maximum shadow distance.
        /// </summary>
        internal static float MaxShadowDistance
        {
            get => s_maxShadowDistance;

            set
            {
                // Clamp value to min and max permitted.
                s_maxShadowDistance = Mathf.Clamp(value, MinMaxShadowDistance, MaxMaxShadowDistance);

                // Set field with updated value (only if game has loaded, e.g. Controller isn't null).
                CameraController controller = Controller;
                if (controller != null)
                {
                    controller.m_maxShadowDistance = value;
                }
            }
        }

        /// <summary>
        /// Applies the current cammera settings to the game camera.
        /// Should only be called once level has loaded.
        /// </summary>
        internal static void ApplySettings()
        {
            // Local reference.
            CameraController controller = Controller;

            // Remove border limts.
            controller.m_unlimitedCamera = true;

            // Set camera minimum and maximum distance from target.
            controller.m_minDistance = MinDistance;
            controller.m_maxDistance = MaxDistance;

            // Set shadow parameters.
            controller.m_minShadowDistance = MinShadowDistance;
            controller.m_maxShadowDistance = MaxShadowDistance;

            // Apply near clip plane.
            NearClipPlane = s_nearClipPlane;

            // Set initial camera position, if we have one.
            if (InitialPosition.IsValid)
            {
                controller.m_targetPosition = InitialPosition.Position;
                controller.m_targetAngle = InitialPosition.Angle;
                controller.m_targetSize = InitialPosition.Size;
                controller.m_targetHeight = InitialPosition.Height;
                controller.m_targetSize = InitialPosition.Size;

                if (MainCamera != null)
                {
                    MainCamera.fieldOfView = InitialPosition.FOV;
                }
                else
                {
                    Logging.Error("unable to locate main camera");
                }
            }
        }

        /// <summary>
        /// Rotates the camera to the nearest multiple of the current X rotation to given increment plus one.
        /// </summary>
        /// <param name="increment">X-rotation increment.</param>
        /// <param name="invert">Set to true to invert rotation direction.</param>
        internal static void RotateX(float increment, bool invert)
        {
            float roundedX = Mathf.Round(Controller.m_targetAngle.x / increment) * increment;

            // Invert is set up to rotate right by default, left inverted.
            Controller.m_targetAngle.x = roundedX + (invert ? increment : -increment);
        }

        /// <summary>
        /// Rotates the camera to the nearest multiple of 15 degrees plus the specified rotation.
        /// </summary>
        /// <param name="increment">Y-rotation increment.</param>
        /// <param name="invert">Set to true to invert rotation direction.</param>
        internal static void RotateY(float increment, bool invert)
        {
            float roundedY = Mathf.Round(Controller.m_targetAngle.y / increment) * increment;

            // Invert is set up to rotate dwon by default, up inverted.
            Controller.m_targetAngle.y = roundedY + (invert ? -increment : increment);
        }

        /// <summary>
        /// Resets the camera rotation.
        /// </summary>
        internal static void ResetRotation()
        {
            // Local reference.
            CameraController controller = Controller;
            controller.m_targetAngle = new Vector2(90f, 90f);
        }
    }
}