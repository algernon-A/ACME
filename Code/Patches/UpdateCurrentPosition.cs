using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;


namespace ACME
{
    /// <summary>
    /// Harmony transpiler to allow camera angles above horizontal.
    /// </summary>
    [HarmonyPatch(typeof(CameraController), "UpdateCurrentPosition")]
    public static class UpdateCurrentPosition
    {
        /// <summary>
        /// Harmony transpiler to remove hardcoded camera tilt distance limit.
        /// Drops instructions to always use m_targetAngle.y instead of factoring in m_maxTiltDistance and m_targetSize.
        /// </summary>
        /// <param name="instructions">Original ILCode</param>
        /// <returns>Patched ILCode</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Instruction parsing.
            IEnumerator<CodeInstruction> instructionsEnumerator = instructions.GetEnumerator();
            CodeInstruction instruction;

            // Status flags.
            bool completed = false;

            // Saved labels.
            List<Label> savedLabels = null;

            // Iterate through all instructions in original method.
            while (instructionsEnumerator.MoveNext())
            {
                // Get next instruction.
                instruction = instructionsEnumerator.Current;

                if (!completed)
                {
                    // Skip any ldc.r4 90s.
                    if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float thisFloat && thisFloat == 90f)
                    {
                        // Record any instruction labels for reattachment to the following ldarg.0.
                        if (instruction.labels != null && instruction.labels.Count > 0)
                        {
                            savedLabels = instruction.labels;
                        }
                        continue;
                    }

                    // If this is an ldarg.0 and we've recorded a saved label, attach it to this.
                    if (instruction.opcode == OpCodes.Ldarg_0 && savedLabels != null)
                    {
                        instruction.labels = savedLabels;
                        savedLabels = null;
                    }

                    // If this is sub, start skipping until the next stloc.0.
                    if (instruction.opcode == OpCodes.Sub)
                    {
                        while (instructionsEnumerator.MoveNext())
                        {
                            // Get instruction.
                            instruction = instructionsEnumerator.Current;
                            if (instruction.opcode == OpCodes.Stloc_0)
                            {
                                // Found our target - set flag and resume flow.
                                completed = true;
                                break;
                            }
                        }
                    }
                }

                // Output instruction.
                yield return instruction;
            }

            // If we got here without finding our target, something went wrong.
            if (!completed)
            {
                Logging.Error("transpiler patching failed for CameraController.UpdateCurrentPosition");
            }
        }
    }
}