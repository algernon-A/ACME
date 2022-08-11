// <copyright file="MoveItUtils.cs" company="algernon (K. Algernon A. Sheppard)">
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
    /// Class to handle integration with Move It mod.
    /// </summary>
    internal static class MoveItUtils
    {
        // Move It methods.
        private static MethodInfo s_miGetTotalBounds;
        private static MethodInfo s_miGetAngle;

        /// <summary>
        /// Moves the current camera position to the last stored Move It position.
        /// </summary>
        internal static void SetPosition()
        {
            // Don't do anything if Move It wasn't reflected.
            if (s_miGetTotalBounds == null || s_miGetAngle == null)
            {
                return;
            }

            // Get Move It bounds.
            if (s_miGetTotalBounds.Invoke(null, new object[] { false, false }) is Bounds miBounds)
            {
                // Don't do anything if bounds haven't been set.
                if (miBounds.size != Vector3.zero)
                {
                    // Move camera to point at centre of Move It selection.
                    CameraController controller = CameraUtils.Controller;
                    if (controller != null)
                    {
                        controller.m_targetPosition = miBounds.center;

                        // Rotate camera to point directly at front of selection.
                        if (s_miGetAngle.Invoke(null, null) is float miAngle)
                        {
                            controller.m_targetAngle.x = ((miAngle * -Mathf.Rad2Deg) + 180) % 360;
                        }
                    }
                    else
                    {
                        Logging.Error("unable to get CameraController for MoveItUtils.SetPosition");
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to find the GetTotalBounds and GetAngle methods of Move It's current action item.
        /// </summary>
        internal static void MoveItReflection()
        {
            Logging.KeyMessage("Attempting to find Move It");

            // Action class.
            Type miAction = Type.GetType("MoveIt.Action,MoveIt", false);
            if (miAction == null)
            {
                Logging.Error("MoveIt.Action not reflected");
                return;
            }

            // GetTotalBounds method.
            s_miGetTotalBounds = miAction.GetMethod("GetTotalBounds", BindingFlags.Static | BindingFlags.Public);
            if (s_miGetTotalBounds == null)
            {
                Logging.Error("MoveIt.Action.GetTotalBounds not reflected");
                return;
            }

            // GetAngle method.
            s_miGetAngle = miAction.GetMethod("GetAngle", BindingFlags.Static | BindingFlags.Public);
            if (s_miGetAngle == null)
            {
                Logging.Error("MoveIt.Action.GetAngle not reflected");
                return;
            }

            Logging.Message("Move It reflection complete");
        }
    }
}