using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;


namespace ACME
{
    /// <summary>
    /// Harmony transpiler to allow camera angles above horizontal.
    /// </summary>
    [HarmonyPatch(typeof(CameraController), "UpdateTargetPosition")]
    public static class UpdateTargetPosition
    {
        /// <summary>
        /// Harmony transpiler to replace hardcoded camera angle limits.
        /// Finds a bound check for 'if < 0 then =0' and replaces 0 with -90.
        /// </summary>
        /// <param name="instructions">Original ILCode</param>
        /// <returns>Patched ILCode</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Vector2 y field info.
            FieldInfo vector2y = typeof(Vector2).GetField(nameof(Vector2.y));

            // TerrainManager.SampleRawHeightSmoothWithWater info.
            MethodInfo waterInfo = typeof(TerrainManager).GetMethod(nameof(TerrainManager.SampleRawHeightSmoothWithWater), new Type[] { typeof(Vector3), typeof(bool), typeof(float) } );


            // Instruction parsing.
            IEnumerator<CodeInstruction> instructionsEnumerator = instructions.GetEnumerator();
            CodeInstruction instruction;

            // Status flags.
            bool completedBounds = false;
            bool completedTerrain = false;

            // Iterate through all instructions in original method.
            while (instructionsEnumerator.MoveNext())
            {
                // Get next instruction.
                instruction = instructionsEnumerator.Current;

                // Don't need to do anything if we've already completed.
                if (!completedBounds)
                {
                    // Is this ldfld float32 [UnityEngine]UnityEngine.Vector2::y?
                    if (instruction.opcode == OpCodes.Ldfld && instruction.LoadsField(vector2y))
                    {
                        // Yes - add it to output.
                        yield return instruction;

                        // Check if next instructio is ldc.r4 0.0.
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
                else if(!completedTerrain)
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
        /// Samples terrain height for camera, based on water bobbing setting.
        /// </summary>
        /// <param name="terrainManager">TerrainManager instance</param>
        /// <param name="worldPos">Camera world position</param>
        /// <param name="timeLerp">Perform Linear Interpolation over time ('bobbing')</param>
        /// <param name="waterOffset">Water offset distance (ignored)</param>
        /// <returns>Applicable terrain height</returns>
        public static float SampleHeight(TerrainManager terrainManager, Vector3 worldPos, bool timeLerp, float waterOffset) => HeightOffset.waterBobbing ? terrainManager.SampleRawHeightSmoothWithWater(worldPos, timeLerp, HeightOffset.TerrainClearance) : terrainManager.SampleRawHeightSmooth(worldPos) + HeightOffset.TerrainClearance;
    }
}