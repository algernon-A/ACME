// <copyright file="HeightOffset.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
	using UnityEngine;
	using ColossalFramework;
	using HarmonyLib;

	/// <summary>
	/// Harmony patch to implement collision check disabling and terrain proximity customization.
	/// </summary>
	[HarmonyPatch(typeof(CameraController), "CalculateCameraHeightOffset")]
    public static class HeightOffset
    {
		// Terrain clearance bounds.
		internal const float MinTerrainClearance = -10f;
		internal const float MaxTerrainClearance = 50f;

		// Collision check filters.
		internal static bool buildingCollision = false;
		internal static bool networkCollision = true;
		internal static bool propCollision = false;
		internal static bool treeCollision = false;
		internal static bool waterBobbing = false;

		// Terrain clearance factor - default is 1.8m above ground (-5m).
		private static float terrainClearanceModifier = -3.2f;

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
				terrainClearanceModifier = -5f + Mathf.Clamp(MinTerrainClearance, value, MaxTerrainClearance);
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
		public static bool Prefix(ref float __result, Vector3 worldPos, float distance)
        {
			// Calculate base offset, based on water bobbing setting.
			float offset = UpdateTargetPosition.SampleHeight(Singleton<TerrainManager>.instance, worldPos, timeLerp: true, TerrainClearance);

			// Boilerplate game code start.
			float a = offset - worldPos.y;
			distance *= 0.45f;
			a = Mathf.Max(a, 0f - distance);
			a += distance * 0.375f * Mathf.Pow(1f + 1f / distance, 0f - a);
			offset = worldPos.y + Mathf.Max(0f, a);
			ItemClass.Availability mode = Singleton<ToolManager>.instance.m_properties.m_mode;
			// Boilerplate game code end.

			// This section is modified gamecode.
			if ((mode & ItemClass.Availability.AssetEditor) == 0)
			{
				worldPos.y -= 5f;
				if (buildingCollision)
				{
					// Building collision check.
					ItemClass.Layer layer = ItemClass.Layer.Default;
					if ((mode & ItemClass.Availability.MapEditor) != 0)
					{
						layer |= ItemClass.Layer.Markers;
					}
					offset = Mathf.Max(offset, Singleton<BuildingManager>.instance.SampleSmoothHeight(worldPos, layer));
				}
				if (networkCollision)
				{
					// Network collision check.
					offset = Mathf.Max(offset, Singleton<NetManager>.instance.SampleSmoothHeight(worldPos));
				}
				if (propCollision)
				{
					// Prop collision check.
					offset = Mathf.Max(offset, Singleton<PropManager>.instance.SampleSmoothHeight(worldPos));
				}
				if (treeCollision)
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