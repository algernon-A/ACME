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
        /// Initializes a new instance of the <see cref="Patcher"/> class.
        /// </summary>
        /// <param name="harmonyID">This mod's unique Harmony identifier.</param>
        public Patcher(string harmonyID)
            : base(harmonyID)
        {
        }

        /// <summary>
        /// Gets the active instance reference.
        /// </summary>
        public static new Patcher Instance
        {
            get
            {
                // Auto-initializing getter.
                if (s_instance == null)
                {
                    s_instance = new Patcher(PatcherMod.Instance.HarmonyID);
                }

                return s_instance as Patcher;
            }
        }
        /// <summary>
        /// Apply all Harmony patches.
        /// </summary>
        public override void PatchAll()
        {
            // Don't do anything if already patched.
            if (!Patched)
            {
                // Ensure Harmony is ready before patching.
                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    Logging.KeyMessage("deploying Harmony patches");

                    // Apply all annotated patches and update flag.
                    Harmony harmonyInstance = new Harmony(HarmonyID);
                    harmonyInstance.PatchAll();
                    Patched = true;

                    // Apply zoom to mouse cursor if set.
                    if (s_zoomToCursorPatched != ModSettings.ZoomToCursor)
                    {
                        PatchZoomToCursor(ModSettings.ZoomToCursor);
                    }

                    // Apply disable follow rotation if set.
                    if (s_disableFollowRotationPatched != ModSettings.DisableFollowRotation)
                    {
                        PatchZoomToCursor(ModSettings.DisableFollowRotation);
                    }
                }
                else
                {
                    Logging.Error("Harmony not ready");
                }
            }
        }

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
                    MethodBase targetMethod = typeof(CameraController).GetMethod("FpsBoosterLateUpdate", BindingFlags.Public | BindingFlags.Instance);
                    if (targetMethod == null)
                    {
                        targetMethod = typeof(CameraController).GetMethod("LateUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                    if (targetMethod == null)
                    {
                        Logging.Error("unable to find FPS patch target method");
                        return;
                    }

                    // Patch method.
                    MethodInfo patchMethod = typeof(FPSPatch).GetMethod(nameof(FPSPatch.LateUpdate));
                    if (patchMethod == null)
                    {
                        Logging.Error("unable to find FPS patch method");
                        return;
                    }

                    Harmony harmonyInstance = new Harmony(HarmonyID);

                    // Apply or remove patches according to flag.
                    if (active)
                    {
                        harmonyInstance.Patch(targetMethod, prefix: new HarmonyMethod(patchMethod));
                    }
                    else
                    {
                        harmonyInstance.Unpatch(targetMethod, patchMethod);
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
                    MethodBase updateTargetMethod = typeof(CameraController).GetMethod("UpdateTargetPosition", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodBase updateTransformMethod = typeof(CameraController).GetMethod("UpdateTransform", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (updateTargetMethod == null || updateTransformMethod == null)
                    {
                        Logging.Error("unable to find mouse zoom cursor patch target method");
                        return;
                    }

                    // Patch method.
                    MethodInfo patchTargetMethod = typeof(ZoomToCursorPatches).GetMethod(nameof(ZoomToCursorPatches.UpdateTargetPrefix));
                    MethodInfo patchTransformMethod = typeof(ZoomToCursorPatches).GetMethod(nameof(ZoomToCursorPatches.UpdateTransformPrefix));

                    if (patchTargetMethod == null || patchTransformMethod == null)
                    {
                        Logging.Error("unable to find mouse zoom cursor patch method");
                        return;
                    }

                    Harmony harmonyInstance = new Harmony(HarmonyID);

                    // Apply or remove patches according to flag.
                    if (active)
                    {
                        harmonyInstance.Patch(updateTargetMethod, prefix: new HarmonyMethod(patchTargetMethod));
                        harmonyInstance.Patch(updateTransformMethod, prefix: new HarmonyMethod(patchTransformMethod));
                    }
                    else
                    {
                        harmonyInstance.Unpatch(updateTargetMethod, patchTargetMethod);
                        harmonyInstance.Unpatch(updateTransformMethod, patchTransformMethod);
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
                    MethodBase targetMethod = typeof(CameraController).GetMethod("FollowTarget", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (targetMethod == null)
                    {
                        Logging.Error("unable to find FollowTarget patch target method");
                        return;
                    }

                    // Patch method.
                    MethodInfo patchMethod = typeof(FollowTargetPatch).GetMethod(nameof(FollowTargetPatch.Transpiler));
                    if (patchMethod == null)
                    {
                        Logging.Error("unable to find FollowTarget transpiler");
                        return;
                    }

                    Harmony harmonyInstance = new Harmony(HarmonyID);

                    // Apply or remove patches according to flag.
                    if (active)
                    {
                        harmonyInstance.Patch(targetMethod, transpiler: new HarmonyMethod(patchMethod));
                    }
                    else
                    {
                        harmonyInstance.Unpatch(targetMethod, patchMethod);
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
    }
}