// <copyright file="ModSettings.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System.IO;
    using System.Xml.Serialization;
    using AlgernonCommons.Keybinding;
    using AlgernonCommons.Patching;
    using AlgernonCommons.XML;
    using UnityEngine;

    /// <summary>
    /// Global mod settings.
    /// </summary>
    [XmlRoot("ACME")]
    public class ModSettings : SettingsXMLBase
    {
        /// <summary>
        /// Settings file name.
        /// </summary>
        [XmlIgnore]
        private static readonly string SettingsFileName = Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "ACME.xml");

        // Private flags.
        [XmlIgnore]
        private static bool _zoomToCursor = false;
        [XmlIgnore]
        private static bool _disableFollowRotation = false;

        /// <summary>
        /// Gets or sets a value indicating whether building collision is enabled.
        /// </summary>
        [XmlElement("BuildingCollision")]
        public bool XMLBuildingCollision { get => HeightOffset.BuildingCollision; set => HeightOffset.BuildingCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether network collision is enabled.
        /// </summary>
        [XmlElement("NetworkCollision")]
        public bool XMLNetworkCollision { get => HeightOffset.NetworkCollision; set => HeightOffset.NetworkCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether prop collision is enabled.
        /// </summary>
        [XmlElement("PropCollision")]
        public bool XMLPropCollision { get => HeightOffset.PropCollision; set => HeightOffset.PropCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether tree collision is enabled.
        /// </summary>
        [XmlElement("TreeCollision")]
        public bool XMLTreeCollision { get => HeightOffset.TreeCollision; set => HeightOffset.TreeCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether water bobbing is enabled.
        /// </summary>
        [XmlElement("WaterBobbing")]
        public bool XMLWaterBobbing { get => UpdateTargetPosition.WaterBobbing; set => UpdateTargetPosition.WaterBobbing = value; }

        /// <summary>
        /// Gets or sets the camera terrain clearance amount, in metres.
        /// </summary>
        [XmlElement("GroundClearance")]
        public float XMLGroundClearance { get => HeightOffset.TerrainClearance; set => HeightOffset.TerrainClearance = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the camera should automatically go to any new disaster.
        /// </summary>
        [XmlElement("FollowDisasters")]
        public bool XMLFollowDisasters { get => FollowDisasterPatch.FollowDisasters; set => FollowDisasterPatch.FollowDisasters = value; }

        /// <summary>
        /// Gets or sets the camera controllers base near clip plane base distance, in metres.
        /// </summary>
        [XmlElement("NearClip")]
        public float XMLNearClip { get => CameraUtils.NearClipPlane; set => CameraUtils.NearClipPlane = value; }

        /// <summary>
        /// Gets or sets the camera speed multiplier.
        /// </summary>
        [XmlElement("SpeedMultiplier")]
        public int XMLSpeedMultiplier { get => (int)UpdateTargetPosition.CameraSpeed; set => UpdateTargetPosition.CameraSpeed = value; }

        /// <summary>
        /// Gets or sets a value indicating whether zoom to mouse cursor is enabled.
        /// </summary>
        [XmlElement("ZoomToCursor")]
        public bool XMLZoomToCursor { get => ZoomToCursor; set => ZoomToCursor = value; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic rotation changes when following a vehicle are suppressed (true) or allowed (false).
        /// </summary>
        [XmlElement("DisableFollowRotation")]
        public bool XMLDisableFollowRotation { get => DisableFollowRotation; set => DisableFollowRotation = value; }

        /// <summary>
        /// Gets or sets the camera controllers minimum shadow distance (shadow sharpness).
        /// </summary>
        [XmlElement("MinShadowDistance")]
        public float XMLMinShadowDistance { get => CameraUtils.MinShadowDistance; set => CameraUtils.MinShadowDistance = value; }

        /// <summary>
        /// Gets or sets the camera controllers maximum shadow distance, in metres.
        /// </summary>
        [XmlElement("MaxShadowDistance")]
        public float XMLMaxShadowDistance { get => CameraUtils.MaxShadowDistance; set => CameraUtils.MaxShadowDistance = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute up movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsUp { get => FPSPatch.AbsUp; set => FPSPatch.AbsUp = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute down movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsDown { get => FPSPatch.AbsDown; set => FPSPatch.AbsDown = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute forward movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsForward { get => FPSPatch.AbsForward; set => FPSPatch.AbsForward = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute back movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsBack { get => FPSPatch.AbsBack; set => FPSPatch.AbsBack = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute left movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsLeft { get => FPSPatch.AbsLeft; set => FPSPatch.AbsLeft = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute right movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsRight { get => FPSPatch.AbsRight; set => FPSPatch.AbsRight = value; }

        /// <summary>
        /// Gets or sets the FPS mod relative forward movement key.
        /// </summary>
        public KeyOnlyBinding FPSRelForward { get => FPSPatch.CameraMoveForward; set => FPSPatch.CameraMoveForward = value; }

        /// <summary>
        /// Gets or sets the FPS mod relative back movement key.
        /// </summary>
        public KeyOnlyBinding FPSRelBack { get => FPSPatch.CameraMoveBackward; set => FPSPatch.CameraMoveBackward = value; }

        /// <summary>
        /// Gets or sets the FPS mod relative left movement key.
        /// </summary>
        public KeyOnlyBinding FPSRelLeft { get => FPSPatch.CameraMoveLeft; set => FPSPatch.CameraMoveLeft = value; }

        /// <summary>
        /// Gets or sets the FPS mod relative right movement key.
        /// </summary>
        public KeyOnlyBinding FPSRelRight { get => FPSPatch.CameraMoveRight; set => FPSPatch.CameraMoveRight = value; }

        /// <summary>
        /// Gets or sets the FPS mod look up key.
        /// </summary>
        public KeyOnlyBinding FPSLookUp { get => FPSPatch.CameraRotateUp; set => FPSPatch.CameraRotateUp = value; }

        /// <summary>
        /// Gets or sets the FPS mod look down key.
        /// </summary>
        public KeyOnlyBinding FPSLookDown { get => FPSPatch.CameraRotateDown; set => FPSPatch.CameraRotateDown = value; }

        /// <summary>
        /// Gets or sets the FPS mod look left key.
        /// </summary>
        public KeyOnlyBinding FPSLookLeft { get => FPSPatch.CameraRotateLeft; set => FPSPatch.CameraRotateLeft = value; }

        /// <summary>
        /// Gets or sets the FPS mod look right key.
        /// </summary>
        public KeyOnlyBinding FPSLookRight { get => FPSPatch.CameraRotateRight; set => FPSPatch.CameraRotateRight = value; }

        /// <summary>
        /// Gets or sets the FPS mod mouse look key.
        /// </summary>
        public KeyOnlyBinding FPSMouseLook { get => FPSPatch.CameraMouseRotate; set => FPSPatch.CameraMouseRotate = value; }

        /// <summary>
        /// Gets or sets the UUI hotkey.
        /// </summary>
        [XmlElement("ToggleKey")]
        public Keybinding ToggleKey { get => UUI.UUIKey.Keybinding; set => UUI.UUIKey.Keybinding = value; }

        /// <summary>
        /// Gets or sets the MoveIt zoom hotkey.
        /// </summary>
        [XmlElement("MoveItZoomKey")]
        public Keybinding MoveItZoomKey { get => UIThreading.MoveItKey; set => UIThreading.MoveItKey = value; }

        /// <summary>
        /// Gets or sets the MoveIt zoom hotkey.
        /// </summary>
        [XmlElement("ResetRotationKey")]
        public Keybinding ResetRotationKey { get => UIThreading.ResetKey; set => UIThreading.ResetKey = value; }

        /// <summary>
        /// Gets or sets the fixed X-rotation hotkey.
        /// </summary>
        [XmlElement("XRotationKey")]
        public KeyOnlyBinding XRotationKey { get => UIThreading.XKey; set => UIThreading.XKey = value; }

        /// <summary>
        /// Gets or sets the fixed Y-rotation hotkey.
        /// </summary>
        [XmlElement("YRotationKey")]
        public KeyOnlyBinding YRotationKey { get => UIThreading.YKey; set => UIThreading.YKey = value; }

        /// <summary>
        /// Gets or sets the FPS mode hotkey.
        /// </summary>
        [XmlElement("FPSModeKey")]
        public Keybinding FPSModeKey { get => UIThreading.FPSModeKey; set => UIThreading.FPSModeKey = value; }

        /// <summary>
        /// Gets or sets FPS turn speed.
        /// </summary>
        [XmlElement("FPSKeyTurnSpeed")]
        public float XMLFPSKeyTurnSpeed { get => FPSPatch.KeyTurnSpeed; set => FPSPatch.KeyTurnSpeed = value; }

        /// <summary>
        /// Gets or sets FPS move speed.
        /// </summary>
        [XmlElement("FPSKeyMoveSpeed")]
        public float XMLFPSKeyMoveSpeed { get => FPSPatch.KeyMoveSpeed; set => FPSPatch.KeyMoveSpeed = value; }

        /// <summary>
        /// Gets or sets FPS mouse turnng speed.
        /// </summary>
        [XmlElement("FPSMouseTurnSpeed")]
        public float XMLFPSMouseTurnSpeed { get => FPSPatch.MouseTurnSpeed; set => FPSPatch.MouseTurnSpeed = value; }

        /// <summary>
        /// Gets or sets the map drag hotkey.
        /// </summary>
        [XmlElement("MapDragKey")]
        public Keybinding XMLMapDragKey { get => MapDragKey; set => MapDragKey = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the map dragging X axis movement is inverted.
        /// </summary>
        [XmlElement("MapDragInvertY")]
        public bool XMLMapDragInvertX { get => MapDragging.InvertXDrag; set => MapDragging.InvertXDrag = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the map dragging Y axis movement is inverted.
        /// </summary>
        [XmlElement("MapDragInvertX")]
        public bool XMLMapDragInvertY { get => MapDragging.InvertYDrag; set => MapDragging.InvertYDrag = value; }

        /// <summary>
        /// Gets or sets map dragging speed.
        /// </summary>
        [XmlElement("MapDragSpeed")]
        public float XMLMapDragSpeed { get => MapDragging.DragSpeed; set => MapDragging.DragSpeed = value; }

        /// <summary>
        /// Gets or sets editor saved positions.
        /// </summary>
        [XmlArray("EditorPositions")]
        [XmlArrayItem("Position")]
        public CameraPositions.SerializedPosition[] XMLEditorPositions { get => CameraPositions.XMLSerialize(); set => CameraPositions.XMLDeserialize(value); }

        /// <summary>
        /// Gets or sets the map dragging hotkey.
        /// </summary>
        [XmlIgnore]
        internal static Keybinding MapDragKey { get; set; } = new Keybinding(KeyCode.Mouse1, false, true, false);

        /// <summary>
        /// Gets or sets a value indicating whether zoom to cursor functionality is enabled.
        /// </summary>
        [XmlIgnore]
        internal static bool ZoomToCursor
        {
            get => _zoomToCursor;

            set
            {
                _zoomToCursor = value;

                // Apply/unapply Harmony patch on value change.
                if (PatcherManager<Patcher>.IsReady)
                {
                    PatcherManager<Patcher>.Instance.PatchZoomToCursor(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether automatic camera rotation is disabled when following a target.
        /// </summary>
        [XmlIgnore]
        internal static bool DisableFollowRotation
        {
            get => _disableFollowRotation;

            set
            {
                _disableFollowRotation = value;

                // Apply/unapply Harmony patch on value change.
                if (PatcherManager<Patcher>.IsReady)
                {
                    PatcherManager<Patcher>.Instance.PatchFollowRotation(value);
                }
            }
        }

        /// <summary>
        /// Loads settings from file.
        /// </summary>
        internal static void Load() => XMLFileUtils.Load<ModSettings>(SettingsFileName);

        /// <summary>
        /// Saves settings to file.
        /// </summary>
        internal static void Save() => XMLFileUtils.Save<ModSettings>(SettingsFileName);
    }
}