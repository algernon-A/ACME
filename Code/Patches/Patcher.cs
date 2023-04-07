// <copyright file="Patcher.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System.Reflection;
    using AlgernonCommons;
    using AlgernonCommons.Patching;
    using CitiesHarmony.API;
    using HarmonyLib;

    /// <summary>
    /// Class to manage the mod's Harmony patches.
    /// </summary>
    public class Patcher : PatcherBase
    {
        // Flags.
        private static bool s_fpsPatched = false;
        private static bool s_zoomToCursorPatched = false;
        private static bool s_disableFollowRotationPatched = false;

        /// <summary>
        /// Applies or unapplies free FPS mode CameraController patch.
        /// </summary>
        /// <param name="active">True to apply patch, false to unapply.</param>
        internal void PatchFPS(bool active)
        {
            // Don't do anything if we're already at the current state.
            if (s_fpsPatched != active)
            {
                // Ensure Harmony is ready before patching.
                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    // Target method: try for FPS Booster LateUpdate first, if that fails, then try vanilla game.
                    MethodInfo targetMethod = AccessTools.Method(typeof(CameraController), "FpsBoosterLateUpdate");
                    if (targetMethod == null)
                    {
                        targetMethod = AccessTools.Method(typeof(CameraController), "LateUpdate");
                    }

                    if (targetMethod == null)
                    {
                        Logging.Error("unable to find FPS patch target method");
                        return;
                    }

                    // Patch method.
                    MethodInfo patchMethod = AccessTools.Method(typeof(FPSPatch), nameof(FPSPatch.LateUpdate));
                    if (patchMethod == null)
                    {
                        Logging.Error("unable to find FPS patch method");
                        return;
                    }

                    Harmony harmonyInstance = new Harmony(HarmonyID);

                    // Apply or remove patches according to flag.
                    if (active)
                    {
                        PrefixMethod(targetMethod, patchMethod);
                    }
                    else
                    {
                        UnpatchMethod(targetMethod, patchMethod);
                    }

                    // Update status flag.
                    s_fpsPatched = active;
                }
                else
                {
                    Logging.Error("Harmony not ready");
                }
            }
        }

        /// <summary>
        /// Applies or unapplies zoom to cursor CameraController patches.
        /// </summary>
        /// <param name="active">True to apply patch, false to unapply.</param>
        internal void PatchZoomToCursor(bool active)
        {
            // Don't do anything if we're already at the current state.
            if (s_zoomToCursorPatched != active)
            {
                // Ensure Harmony is ready before patching.
                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    // Target methods.
                    MethodInfo updateTargetMethod = AccessTools.Method(typeof(CameraController), "UpdateTargetPosition");
                    MethodInfo updateTransformMethod = AccessTools.Method(typeof(CameraController), "UpdateTransform");

                    if (updateTargetMethod == null || updateTransformMethod == null)
                    {
                        Logging.Error("unable to find mouse zoom cursor patch target method");
                        return;
                    }

                    // Patch method.
                    MethodInfo patchTargetMethod = AccessTools.Method(typeof(ZoomToCursorPatches), nameof(ZoomToCursorPatches.UpdateTargetPrefix));
                    MethodInfo patchTransformMethod = AccessTools.Method(typeof(ZoomToCursorPatches), nameof(ZoomToCursorPatches.UpdateTransformPrefix));

                    if (patchTargetMethod == null || patchTransformMethod == null)
                    {
                        Logging.Error("unable to find mouse zoom cursor patch method");
                        return;
                    }

                    Harmony harmonyInstance = new Harmony(HarmonyID);

                    // Apply or remove patches according to flag.
                    if (active)
                    {
                        PrefixMethod(updateTargetMethod, patchTargetMethod);
                        PrefixMethod(updateTransformMethod, patchTransformMethod);
                    }
                    else
                    {
                        UnpatchMethod(updateTargetMethod, patchTargetMethod);
                        UnpatchMethod(updateTransformMethod, patchTransformMethod);
                    }

                    // Update status flag.
                    s_zoomToCursorPatched = active;
                }
                else
                {
                    Logging.Error("Harmony not ready");
                }
            }
        }

        /// <summary>
        /// Applies or unapplies transpiler to disable rotation changes while following a target.
        /// </summary>
        /// <param name="active">True to apply patch, false to unapply.</param>
        internal void PatchFollowRotation(bool active)
        {
            // Don't do anything if we're already at the current state.
            if (s_disableFollowRotationPatched != active)
            {
                // Ensure Harmony is ready before patching.
                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    // Target method.
                    MethodInfo targetMethod = AccessTools.Method(typeof(CameraController), "FollowTarget");
                    if (targetMethod == null)
                    {
                        Logging.Error("unable to find FollowTarget patch target method");
                        return;
                    }

                    // Patch method.
                    MethodInfo patchMethod = AccessTools.Method(typeof(FollowTargetPatch), nameof(FollowTargetPatch.Transpiler));
                    if (patchMethod == null)
                    {
                        Logging.Error("unable to find FollowTarget transpiler");
                        return;
                    }

                    Harmony harmonyInstance = new Harmony(HarmonyID);

                    // Apply or remove patches according to flag.
                    if (active)
                    {
                        PrefixMethod(targetMethod, patchMethod);
                    }
                    else
                    {
                        UnpatchMethod(targetMethod, patchMethod);
                    }

                    // Update status flag.
                    s_disableFollowRotationPatched = active;
                }
                else
                {
                    Logging.Error("Harmony not ready");
                }
            }
        }

        /// <summary>
        /// Peforms any additional actions (such as custom patching) after PatchAll is called.
        /// </summary>
        /// <param name="harmonyInstance">Haromny instance for patching.</param>
        protected override void OnPatchAll(Harmony harmonyInstance) => PatchZoomToCursor(ModSettings.ZoomToCursor);
    }
}