using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;

using ColossalFramework;


namespace ACME
{
    /// <summary>
    /// Harmony transpiler to allow camera angles above horizontal and implement variable movement speed multipliers.
    /// </summary>
    [HarmonyPatch(typeof(CameraController), "UpdateTargetPosition")]
    public static class UpdateTargetPosition
    {
        // Minumum and maximum speed multiplier.
        internal const float MinCameraSpeed = 1f;
        internal const float MaxCameraSpeed = 70f;

        // Camera speed multiplier.
        private static float cameraSpeed = 15f;

        // Camera dragging status.
        private static bool isDragging = false;


        /// <summary>
        /// Camera speed multiplier.
        /// </summary>
        internal static float CameraSpeed { get => cameraSpeed; set => cameraSpeed = Mathf.Clamp(value, MinCameraSpeed, MaxCameraSpeed); }


        /// <summary>
        /// Harmony transpiler to replace hardcoded camera angle limits and implement variable movement speed multipliers.
        /// Finds a bound check for 'if < 0 then =0' and replaces 0 with -90 (for camera angle) and inserts calls to custom movement multiplier methods.
        /// </summary>
        /// <param name="instructions">Original ILCode</param>
        /// <returns>Patched ILCode</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Vector2 y field info.
            FieldInfo vector2y = typeof(Vector2).GetField(nameof(Vector2.y));

            // TerrainManager.SampleRawHeightSmoothWithWater info.
            MethodInfo waterInfo = typeof(TerrainManager).GetMethod(nameof(TerrainManager.SampleRawHeightSmoothWithWater), new Type[] { typeof(Vector3), typeof(bool), typeof(float) });


            // Instruction parsing.
            IEnumerator<CodeInstruction> instructionsEnumerator = instructions.GetEnumerator();
            CodeInstruction instruction;

            // Status flags.
            bool completedBounds = false;
            bool completedTerrain = false;


            // Method calls for flagging insertion of custom multiplier code.
            MethodInfo handleKeyEvents = typeof(CameraController).GetMethod("HandleKeyEvents", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo handleScrollEvents = typeof(CameraController).GetMethod("HandleScrollWheelEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo handleMouseEvents = typeof(CameraController).GetMethod("HandleMouseEvents", BindingFlags.NonPublic | BindingFlags.Instance);

            // Custom multiplier methods to insert.
            MethodInfo keyMultiplier = typeof(UpdateTargetPosition).GetMethod(nameof(UpdateTargetPosition.KeyMultiplier), BindingFlags.Public | BindingFlags.Static);
            MethodInfo scrollMultiplier = typeof(UpdateTargetPosition).GetMethod(nameof(UpdateTargetPosition.ScrollMultiplier), BindingFlags.Public | BindingFlags.Static);
            MethodInfo mouseDrag = typeof(UpdateTargetPosition).GetMethod(nameof(UpdateTargetPosition.MouseDrag), BindingFlags.Public | BindingFlags.Static);

            // Iterate through all instructions in original method.
            while (instructionsEnumerator.MoveNext())
            {
                // Get next instruction.
                instruction = instructionsEnumerator.Current;

                // Don't need to do anything if we've already completed.
                if (!completedBounds)
                {
                    // Check for call instruction.
                    if (instruction.opcode == OpCodes.Call)
                    {
                        // Found call - check for method operands matching our targets.
                        if (instruction.operand is MethodInfo method)
                        {
                            if (method == handleMouseEvents)
                            {
                                Logging.Message("inserting custom HandleMouseEvents call");
                                yield return new CodeInstruction(OpCodes.Ldarg_0);
                                yield return new CodeInstruction(OpCodes.Call, mouseDrag);
                            }
                            else if (method == handleKeyEvents)
                            {
                                // Found call to HandleKeyEvents - insert call to our custom method immediately before the game call.
                                Logging.Message("adding pre-HandleKeyEvents multiplier call");

                                // Base multiplier is already on stack; just need to add instance reference.  Custom method will replace base multiplier on stack with updated one.
                                yield return new CodeInstruction(OpCodes.Ldarg_0);
                                yield return new CodeInstruction(OpCodes.Call, keyMultiplier);
                            }
                            else if (method == handleScrollEvents)
                            {
                                // Found call to HandleScrollWheelEvent - insert call to our custom method immediately before the game call.
                                Logging.Message("adding pre-HandleScrollWheelEvent multiplier call");

                                // Base multiplier is already on stack; just need to add instance reference.  Custom method will replace base multiplier on stack with updated one.
                                yield return new CodeInstruction(OpCodes.Ldarg_0);
                                yield return new CodeInstruction(OpCodes.Call, scrollMultiplier);
                            }
                        }
                    }
                    // Is this ldfld float32 [UnityEngine]UnityEngine.Vector2::y?
                    else if (instruction.opcode == OpCodes.Ldfld && instruction.LoadsField(vector2y))
                    {
                        // Yes - add it to output.
                        yield return instruction;

                        // Check if next instruction is ldc.r4 0.0.
                        if (instructionsEnumerator.MoveNext())
                        {
                            instruction = instructionsEnumerator.Current;
                            if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float checkValue && checkValue == 0.0f)
                            {
                                // ldc.r4 0.0 found - change operand to -90 (this is conditional check).
                                instruction.operand = -90f;

                                // Iterate forwards to next ldc.r4 0.0f and set it to -90f (this is the application of the bounds check).
                                while (instructionsEnumerator.MoveNext())
                                {
                                    // Add current instruction to output before fetching new one.
                                    yield return instruction;
                                    instruction = instructionsEnumerator.Current;
                                    if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float checkValue2 && checkValue2 == 0.0f)
                                    {
                                        instruction.operand = -90f;

                                        // Success - all done here, break out of inner while loop and fall through to outer.
                                        completedBounds = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!completedTerrain)
                {
                    // Only start looking for call to TerrainManager.SampleRawHeightSmoothWithWater once we've finished bounds checks.
                    if (instruction.Calls(waterInfo))
                    {
                        // Replace call to TerrainManager.SampleRawHeightSmoothWithWater with a call to our own SampleHeight method (implements water bobbing toggle).
                        instruction = new CodeInstruction(OpCodes.Call, typeof(UpdateTargetPosition).GetMethod(nameof(UpdateTargetPosition.SampleHeight)));
                        completedTerrain = true;
                    }
                }

                // Output instruction.
                yield return instruction;
            }

            // If we got here without finding both our targets, something went wrong.
            if (!completedBounds && !completedTerrain)
            {
                Logging.Error("transpiler patching failed for CameraController.UpdateTargetPosition");
            }
        }


        /// <summary>
        /// Calculates the multiplier of key-based camera movement where appropriate.
        /// </summary>
        /// <param name="baseMult">Base speed multiplier (calculated by game)</param>
        /// <param name="instance">CameraController instance reference</param>
        /// <returns>Speed multiplier for calculating target position update</returns>
        public static float KeyMultiplier(float baseMult, CameraController instance) => (instance.m_currentSize > 43f) ? baseMult : (baseMult * cameraSpeed / instance.m_currentSize);


        /// Calculates the multiplier of scroll whell-based camera movement where appropriate.
        /// </summary>
        /// <param name="baseMult">Base speed multiplier (calculated by game)</param>
        /// <param name="instance">CameraController instance reference</param>
        /// <returns>Speed multiplier for calculating target position update</returns>
        public static float ScrollMultiplier(float baseMult, CameraController instance) => (instance.m_currentSize > 5f) ? baseMult : (baseMult * cameraSpeed / 2 / instance.m_currentSize);


        /// <summary>
        /// Samples terrain height for camera, based on water bobbing setting.
        /// </summary>
        /// <param name="terrainManager">TerrainManager instance</param>
        /// <param name="worldPos">Camera world position</param>
        /// <param name="timeLerp">Perform Linear Interpolation over time ('bobbing')</param>
        /// <param name="waterOffset">Water offset distance (ignored)</param>
        /// <returns>Applicable terrain height</returns>
        public static float SampleHeight(TerrainManager terrainManager, Vector3 worldPos, bool timeLerp, float waterOffset) => HeightOffset.waterBobbing ? terrainManager.SampleRawHeightSmoothWithWater(worldPos, timeLerp, HeightOffset.TerrainClearance) : terrainManager.SampleRawHeightSmooth(worldPos) + HeightOffset.TerrainClearance;


        /// <summary>
        /// Implements 
        /// </summary>
        /// <param name="controller"></param>
        public static void MouseDrag(CameraController controller)
        {
            // Is the rotate button pressed?
            if (ModSettings.mapDragKey.IsPressed())
            {
                // Get screen mouse movement direction.
                Vector3 direction = new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y"));

                // Speed multiplier based on height.
                float heightSpeedFactor = controller.m_currentSize / 10f;
                if (heightSpeedFactor < 5f)
                {
                    heightSpeedFactor = 5f;
                }
                direction *= heightSpeedFactor * 0.3f;

                // Convert screen movement to relative to camera rotation.
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, controller.transform.right);
                controller.m_targetPosition += quaternion * direction;

                // Lock cursor while dragging and set active flag height.
                Cursor.lockState = CursorLockMode.Locked;
                isDragging = true;
            }
            else if (isDragging)
            {
                // We're dragging but the button is now released - unlock cursor and clear status.
                Cursor.lockState = CursorLockMode.None;
                isDragging = false;
            }
        }
    }
}