// <copyright file="UpdateTransform.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;

    /// <summary>
    /// Harmony transpiler to compensate for .
    /// </summary>
    [HarmonyPatch(typeof(CameraController), "UpdateTransform")]
    public static class UpdateTransform
    {
        /// <summary>
        /// Harmony transpiler to restore vanilla default shadow calculations (altered by changing m_maxDistance).
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <returns>Patched ILCode.</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // State counter.
            int maxDistanceCount = 0;

            // Target field.
            FieldInfo m_maxDistanceInfo = AccessTools.Field(typeof(CameraController), nameof(CameraController.m_maxDistance));

            // Iterate through instructions.
            IEnumerator<CodeInstruction> instructionEnumerator = instructions.GetEnumerator();
            while (instructionEnumerator.MoveNext())
            {
                CodeInstruction instruction = instructionEnumerator.Current;

                switch (maxDistanceCount)
                {
                    case 0:
                        // Skip first m_maxDistanceInfo load (camera distance).
                        if (instruction.LoadsField(m_maxDistanceInfo))
                        {
                            maxDistanceCount = 1;
                        }

                        break;

                    case 1:
                        // Look for second m_maxDistanceInfo load (shadow calculations).
                        if (instruction.opcode == OpCodes.Ldarg_0)
                        {
                            // If this is our target, we need to drop the leading ldarg.0 as well - so looking at two instructions at once.
                            instructionEnumerator.MoveNext();
                            CodeInstruction nextInstruction = instructionEnumerator.Current;

                            // Replace m_maxDistanceInfo with vanilla default figure (3000f), dropping leading ldarg.0.
                            if (nextInstruction.LoadsField(m_maxDistanceInfo))
                            {
                                instruction = new CodeInstruction(OpCodes.Ldc_R4, 3000f);
                                maxDistanceCount = 2;
                            }
                            else
                            {
                                // Not a target - retain original code (both instrucations).
                                yield return instruction;
                                yield return nextInstruction;
                                continue;
                            }
                        }

                        break;
                }

                yield return instruction;
            }
        }
    }
}