using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;


namespace ACME
{
    /// <summary>
    /// Harmony patch to implement enabling/disabling of automatic camera rotation changes when following a target.
    /// </summary>
    public static class FollowTargetPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            Logging.Message("transpiling ", original.DeclaringType, ":", original.Name);

            // Status flags.
            bool skipping = false;
            bool skipped = false;

            // Reflection to get fields used to determine code skipping boundaries.
            FieldInfo m_currentTargetRotation = typeof(CameraController).GetField("m_currentTargetRotation", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo m_targetPosition = typeof(CameraController).GetField("m_targetPosition", BindingFlags.Public | BindingFlags.Instance);

            // Instruction enumerator.
            IEnumerator<CodeInstruction> instructionsEnumerator = instructions.GetEnumerator();

            // Iterate through each instruction in original code.
            while (instructionsEnumerator.MoveNext())
            {
                // Get next instruction.
                CodeInstruction instruction = instructionsEnumerator.Current;

                // Don't bother doing anything fancy if we've already skipped the relevant instructions.
                if (!skipped)
                {
                    // Are we skipping instructions currently?
                    if (skipping)
                    {
                        // Yes - is this stfld m_currentTargetRotation?
                        if (instruction.opcode == OpCodes.Stfld && instruction.operand == m_currentTargetRotation)
                        {
                            // Yes - that's our cue to resume after this one.
                            skipping = false;
                            skipped = true;
                        }

                        // Skip current instruction.
                        continue;
                    }
                    else if (instruction.opcode == OpCodes.Stfld && instruction.operand == m_targetPosition)
                    {
                        // Encountered stfld m_targetPosition; start skipping after this instruction.
                        skipping = true;
                    }
                }

                // Output this instruction.
                yield return instruction;
            }
        }
    }
}