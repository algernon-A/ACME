// <copyright file="ZoomToCursorPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using UnityEngine;
    using ColossalFramework;
    using ColossalFramework.Math;

    /// <summary>
    /// Harmony patches to implement zoom to mouse cursor functionality.
    /// </summary>
    public static class ZoomToCursorPatches
    {
        // Frame starting currentSize.
        private static float s_oldCurrentSize;

        /// <summary>
        /// Harmony Prefix patch for CameraController.UpdateTargetPosition to record frame starting currentSize.
        /// </summary>
        /// <param name="__instance">CameraController instance.</param>
        public static void UpdateTargetPrefix(CameraController __instance)
        {
            // Simply recording the original is quicker than reverse engineering the value or accessing private members.
            s_oldCurrentSize = __instance.m_currentSize;
        }


        /// <summary>
        /// Harmony Prefix patch for CameraController.UpdateTransform, to implement mouse cursor zooming.
        /// </summary>
        /// <param name="__instance">CameraController instance.</param>
        public static void UpdateTransformPrefix(CameraController __instance)
        {
            // Try basic raycast.
            Vector3 output = new Vector3();
            if (s_oldCurrentSize != 0f && s_oldCurrentSize != __instance.m_currentSize && RayCast(ref output))
            {
                // Raycast successful - calculate position offset based on zoom fraction (between current and ray hit positions).
                float zoomScale = 1f - __instance.m_currentSize / s_oldCurrentSize;
                Vector3 offset = zoomScale * (output - __instance.m_currentPosition);

                // Maintain y coordination (adjusts for application of currentSize and avoids 'bouncing').
                offset.y = 0f;

                // Adjust target and current positions.
                __instance.m_targetPosition += offset;
                __instance.m_currentPosition += offset;
            }
        }
        
        /// <summary>
        /// Basic terrain raycasting for zoom to cursor function.
        /// </summary>
        /// <param name="hitPos">Raycast hit position.</param>
        /// <returns>True if terrain raycast hit, false otherwise.</returns>
        private static bool RayCast(ref Vector3 hitPos)
        {
            // Convert mouse cursor location to ray.
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayLength = Camera.main.farClipPlane;

            // Raycast setup.
            Vector3 direction = mouseRay.direction.normalized;
            Vector3 rayVector = mouseRay.origin + (direction * rayLength);
            Segment3 ray = new Segment3(mouseRay.origin, rayVector);

            // Check for terrain hit.
            if (Singleton<TerrainManager>.instance.RayCast(ray, out hitPos))
            {
                // Terrain was hit if the terrain raycast distance (plus margin of 100) is less than our ray length.
                float terrainDistance = Vector3.Distance(hitPos, mouseRay.origin) + 100f;
                return terrainDistance < rayLength;
            }

            // If we got here, we didn't get a terrain hit; return false.
            return false;
        }
    }
}
