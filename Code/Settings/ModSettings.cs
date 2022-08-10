namespace ACME
{
    using System.IO;
    using System.Xml.Serialization;
    using AlgernonCommons.Keybinding;
    using AlgernonCommons.XML;
    using UnityEngine;

    /// <summary>
    /// Global mod settings.
    /// </summary>
    /// 
    [XmlRoot("ACME")]
    public class ModSettings : SettingsXMLBase
    {
        [XmlIgnore]
        internal static Keybinding mapDragKey = new Keybinding(KeyCode.Mouse1, false, true, false);

        // Private flags.
        [XmlIgnore]
        private static bool _zoomToCursor = false;
        [XmlIgnore]
        private static bool _disableFollowRotation = false;

        /// <summary>
        /// Settings file name.
        /// </summary>
        [XmlIgnore]
        private static readonly string SettingsFileName = Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "ACME.xml");

        /// <summary>
        /// Gets or sets a value indicating whether building collision is enabled.
        /// </summary>
        [XmlElement("BuildingCollision")]
        public bool XMLBuildingCollision { get => HeightOffset.buildingCollision; set => HeightOffset.buildingCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether network collision is enabled.
        /// </summary>
        [XmlElement("NetworkCollision")]
        public bool XMLNetworkCollision { get => HeightOffset.networkCollision; set => HeightOffset.networkCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether prop collision is enabled.
        /// </summary>
        [XmlElement("PropCollision")]
        public bool XMLPropCollision { get => HeightOffset.propCollision; set => HeightOffset.propCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether tree collision is enabled.
        /// </summary>
        [XmlElement("TreeCollision")]
        public bool XMLTreeCollision { get => HeightOffset.treeCollision; set => HeightOffset.treeCollision = value; }

        /// <summary>
        /// Gets or sets a value indicating whether water bobbing is enabled.
        /// </summary>
        [XmlElement("WaterBobbing")]
        public bool XMLWaterBobbing { get => HeightOffset.waterBobbing; set => HeightOffset.waterBobbing = value; }

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
        /// Gets or sets the FPS mod absolute up movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsUp { get => FPSPatch.s_absUp; set => FPSPatch.s_absUp = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute down movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsDown { get => FPSPatch.s_absDown; set => FPSPatch.s_absDown = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute forward movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsForward { get => FPSPatch.s_absForward; set => FPSPatch.s_absForward = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute back movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsBack { get => FPSPatch.s_absBack; set => FPSPatch.s_absBack = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute left movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsLeft { get => FPSPatch.s_absLeft; set => FPSPatch.s_absLeft = value; }

        /// <summary>
        /// Gets or sets the FPS mod absolute right movement key.
        /// </summary>
        public KeyOnlyBinding FPSAbsRight { get => FPSPatch.s_absRight; set => FPSPatch.s_absRight = value; }

        /// <summary>
        /// Gets or sets the FPS mod relative forward movement key.
        /// </summary>
        public KeyOnlyBinding FPSRelForward { get => FPSPatch.s_cameraMoveForward; set => FPSPatch.s_cameraMoveForward = value; }

        /// <summary>
        /// Gets or sets the FPS mod relative back movement key.
        /// </summary>
        public KeyOnlyBinding FPSRelBack { get => FPSPatch.s_cameraMoveBackward; set => FPSPatch.s_cameraMoveBackward = value; }

        /// <summary>
        /// Gets or sets the FPS mod relative left movement key.
        /// </summary>
        public KeyOnlyBinding FPSRelLeft { get => FPSPatch.s_cameraMoveLeft; set => FPSPatch.s_cameraMoveLeft = value; }

        /// <summary>
        /// Gets or sets the FPS mod relative right movement key.
        /// </summary>
        public KeyOnlyBinding FPSRelRight { get => FPSPatch.s_cameraMoveRight; set => FPSPatch.s_cameraMoveRight = value; }

        /// <summary>
        /// Gets or sets the FPS mod look up key.
        /// </summary>
        public KeyOnlyBinding FPSLookUp { get => FPSPatch.s_cameraRotateUp; set => FPSPatch.s_cameraRotateUp = value; }

        /// <summary>
        /// Gets or sets the FPS mod look down key.
        /// </summary>
        public KeyOnlyBinding FPSLookDown { get => FPSPatch.s_cameraRotateDown; set => FPSPatch.s_cameraRotateDown = value; }

        /// <summary>
        /// Gets or sets the FPS mod look left key.
        /// </summary>
        public KeyOnlyBinding FPSLookLeft { get => FPSPatch.s_cameraRotateLeft; set => FPSPatch.s_cameraRotateLeft = value; }

        /// <summary>
        /// Gets or sets the FPS mod look right key.
        /// </summary>
        public KeyOnlyBinding FPSLookRight { get => FPSPatch.s_cameraRotateRight; set => FPSPatch.s_cameraRotateRight = value; }

        /// <summary>
        /// Gets or sets the FPS mod mouse look key.
        /// </summary>
        public KeyOnlyBinding FPSMouseLook{ get => FPSPatch.s_cameraMouseRotate; set => FPSPatch.s_cameraMouseRotate = value; }

        /// <summary>
        /// Gets or sets the UUI hotkey.
        /// </summary>
        [XmlElement("ToggleKey")]
        public Keybinding ToggleKey { get => UUI.uuiKey.Keybinding; set => UUI.uuiKey.Keybinding = value; }


        /// <summary>
        /// Gets or sets the MoveIt zoom hotkey.
        /// </summary>
        [XmlElement("MoveItZoomKey")]
        public Keybinding MoveItZoomKey { get => UIThreading.moveItKey; set => UIThreading.moveItKey = value; }

        /// <summary>
        /// Gets or sets the FPS mode hotkey.
        /// </summary>
        [XmlElement("FPSModeKey")]
        public Keybinding FPSModeKey { get => UIThreading.fpsKey; set => UIThreading.fpsKey = value; }

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
        public Keybinding MapDragKey { get => mapDragKey; set => mapDragKey = value; }

        /// <summary>
        /// Gets or sets a vlaue indicating whether the map dragging X axis movement is inverted.
        /// </summary>
        [XmlElement("MapDragInvertY")]
        public bool XMLMapDragInvertX { get => MapDragging.InvertXDrag; set => MapDragging.InvertXDrag = value; }

        /// <summary>
        /// Gets or sets a vlaue indicating whether the map dragging Y axis movement is inverted.
        /// </summary>
        [XmlElement("MapDragInvertX")]
        public bool XMLMapDragInvertY { get => MapDragging.InvertYDrag; set => MapDragging.InvertYDrag = value; }

        /// <summary>
        /// Gets or sets map dragging speed.
        /// </summary>
        [XmlElement("MapDragSpeed")]
        public float XMLMapDragSpeed { get => MapDragging.DragSpeed; set => MapDragging.DragSpeed = value; }

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
                Patcher.Instance.PatchZoomToCursor(value);
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
                Patcher.Instance.PatchFollowRotation(value);
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