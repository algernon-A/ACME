using System.Reflection;
using HarmonyLib;
using CitiesHarmony.API;


namespace ACME
{
    /// <summary>
    /// Class to manage the mod's Harmony patches.
    /// </summary>
    public static class Patcher
    {
        // Unique harmony identifier.
        private const string harmonyID = "com.github.algernon-A.csl.cam";

        // Flags.
        internal static bool Patched => _patched;
        private static bool _patched = false;
        private static bool fpsPatched = false, zoomToCursorPatched = false, disableFollowRotationPatched = false;


        /// <summary>
        /// Apply all Harmony patches.
        /// </summary>
        public static void PatchAll()
        {
            // Don't do anything if already patched.
            if (!_patched)
            {
                // Ensure Harmony is ready before patching.
                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    Logging.KeyMessage("deploying Harmony patches");

                    // Apply all annotated patches and update flag.
                    Harmony harmonyInstance = new Harmony(harmonyID);
                    harmonyInstance.PatchAll();
                    _patched = true;

                    // Apply zoom to mouse cursor if set.
                    if (zoomToCursorPatched != ModSettings.ZoomToCursor)
                    {
                        PatchZoomToCursor(ModSettings.ZoomToCursor);
                    }

                    // Apply disable follow rotation if set.
                    if (disableFollowRotationPatched != ModSettings.DisableFollowRotation)
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
        /// Remove all Harmony patches.
        /// </summary>
        public static void UnpatchAll()
        {
            // Only unapply if patches appplied.
            if (_patched)
            {
                Logging.KeyMessage("reverting Harmony patches");

                // Unapply patches, but only with our HarmonyID.
                Harmony harmonyInstance = new Harmony(harmonyID);
                harmonyInstance.UnpatchAll(harmonyID);
                _patched = false;
            }
        }


        /// <summary>
        /// Applies or unapplies free FPS mode CameraController patch.
        /// </summary>
        /// <param name="active">True to apply patch, false to unapply</param>
        internal static void PatchFPS(bool active)
        {
            // Don't do anything if we're already at the current state.
            if (fpsPatched != active)
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

                    Harmony harmonyInstance = new Harmony(harmonyID);

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
                    fpsPatched = active;
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
        /// <param name="active">True to apply patch, false to unapply</param>
        internal static void PatchZoomToCursor(bool active)
        {
            // Don't do anything if we're already at the current state.
            if (zoomToCursorPatched != active)
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

                    Harmony harmonyInstance = new Harmony(harmonyID);

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
                    zoomToCursorPatched = active;
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
        /// <param name="active">True to apply patch, false to unapply</param>
        internal static void PatchFollowRotation(bool active)
        {
            // Don't do anything if we're already at the current state.
            if (disableFollowRotationPatched != active)
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

                    Harmony harmonyInstance = new Harmony(harmonyID);

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
                    disableFollowRotationPatched = active;
                }
                else
                {
                    Logging.Error("Harmony not ready");
                }
            }
        }
    }
}