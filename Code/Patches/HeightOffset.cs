// <copyright file="HeightOffset.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using ColossalFramework;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmony patch to implement collision check disabling and terrain proximity customization.
    /// </summary>
    [HarmonyPatch(typeof(CameraController), "CalculateCameraHeightOffset")]
    public static class HeightOffset
    {
        /// <summary>
        /// Minimum terrain clearance.
        /// </summary>
        internal const float MinTerrainClearance = -10f;

        /// <summary>
        /// Maximum terrain clearance.
        /// </summary>
        internal const float MaxTerrainClearance = 50f;

        // Collision check filters.
        private static bool s_buildingCollision = false;
        private static bool s_networkCollision = true;
        private static bool s_propCollision = false;
        private static bool s_treeCollision = false;

        // Terrain clearance factor - default is 1.8m above ground (-5m).
        private static float terrainClearanceModifier = -3.2f;

        /// <summary>
        /// Gets or sets a value indicating whether building collision is enabled.
        /// </summary>
        internal static bool BuildingCollision { get => s_buildingCollision; set => s_buildingCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether network collision is enabled.
        /// </summary>
        internal static bool NetworkCollision { get => s_networkCollision; set => s_networkCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether prop collision is enabled.
        /// </summary>
        internal static bool PropCollision { get => s_propCollision; set => s_propCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether tree collision is enabled.
        /// </summary>
        internal static bool TreeCollision { get => s_treeCollision; set => s_treeCollision = value; }

        /// <summary>
        /// Gets or sets the camera terrain clearance amount, in metres.
        /// </summary>
        internal static float TerrainClearance
        {
            // Internal terrain clearance factor starts at -5f.
            get => terrainClearanceModifier + 5f;

            set
            {
                // Internal terrain clearance factor starts at -5f.
                terrainClearanceModifier = -5f + Mathf.Clamp(value, MinTerrainClearance, MaxTerrainClearance);
            }
        }

        /// <summary>
        /// Pre-emptive Harmony Prefix patch to CameraController.CalculateCameraHeightOffset.
        /// Implements selective collision checks and overrides terrain proximity limit.
        /// </summary>
        /// <param name="__result">Original method result.</param>
        /// <param name="worldPos">Camera world position.</param>
        /// <param name="distance">Target distance.</param>
        /// <returns>Always false (never execute original method).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
        public static bool Prefix(ref float __result, Vector3 worldPos, float distance)
        {
            // Calculate base offset, based on water bobbing setting.
            float offset = UpdateTargetPosition.SampleHeight(Singleton<TerrainManager>.instance, worldPos, timeLerp: true, TerrainClearance);

            // Boilerplate game code start.
            float a = offset - worldPos.y;
            distance *= 0.45f;
            a = Mathf.Max(a, 0f - distance);
            a += distance * 0.375f * Mathf.Pow(1f + (1f / distance), 0f - a);
            offset = worldPos.y + Mathf.Max(0f, a);
            ItemClass.Availability mode = Singleton<ToolManager>.instance.m_properties.m_mode;

            // Boilerplate game code end.
            // This section is modified gamecode.
            if ((mode & ItemClass.Availability.AssetEditor) == 0)
            {
                worldPos.y -= 5f;
                if (s_buildingCollision)
                {
                    // Building collision check.
                    ItemClass.Layer layer = ItemClass.Layer.Default;
                    if ((mode & ItemClass.Availability.MapEditor) != 0)
                    {
                        layer |= ItemClass.Layer.Markers;
                    }

                    offset = Mathf.Max(offset, Singleton<BuildingManager>.instance.SampleSmoothHeight(worldPos, layer));
                }

                if (s_networkCollision)
                {
                    // Network collision check.
                    offset = Mathf.Max(offset, Singleton<NetManager>.instance.SampleSmoothHeight(worldPos));
                }

                if (s_propCollision)
                {
                    // Prop collision check.
                    offset = Mathf.Max(offset, Singleton<PropManager>.instance.SampleSmoothHeight(worldPos));
                }

                if (s_treeCollision)
                {
                    // Tree collision check.
                    offset = Mathf.Max(offset, Singleton<TreeManager>.instance.SampleSmoothHeight(worldPos));
                }

                worldPos.y += 5f;
            }

            // End modified game code.
            // Adust result with our custom terrain clearance modifier.
            __result = offset - worldPos.y + terrainClearanceModifier;

            // Never execute original method.
            return false;
        }
    }
}