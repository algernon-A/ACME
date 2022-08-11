// <copyright file="FollowDisasterPatch.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System.Runtime.CompilerServices;
    using HarmonyLib;

    /// <summary>
    /// Harmony patch to implement enabling/disabling of automatic disaster following.
    /// </summary>
    [HarmonyPatch(typeof(DisasterManager), nameof(DisasterManager.FollowDisaster))]
    public static class FollowDisasterPatch
    {
        // Collision check filters.
        private static bool s_followDisasters = true;

        /// <summary>
        /// Gets or sets a value indicating whether the camera should automatically go to any new disaster.
        /// </summary>
        internal static bool FollowDisasters { get => s_followDisasters; set => s_followDisasters = value; }

        /// <summary>
        /// Pre-emptive Harmony Prefix patch to DisasterManager.FollowDisaster.
        /// SImply enables/disables execution of original method based on setting.
        /// </summary>
        /// <returns>True (execute original method) or false (don't execute original method) based on followDisasters setting.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool Prefix() => s_followDisasters;
    }
}