using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using ColossalFramework;


namespace ACME
{
    /// <summary>
    /// Global mod settings.
    /// </summary>
    /// 
    [XmlRoot("ACME")]
    public class ModSettings
    {
        // Settings file name.
        [XmlIgnore]
        private static readonly string SettingsFileName = Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "ACME.xml");

        /// <summary>
        /// Language setting.
        /// </summary>
        [XmlElement("Language")]
        public string XMLLanguage { get => Translations.CurrentLanguage; set => Translations.CurrentLanguage = value; }


        /// <summary>
        /// Building collision enabled/disabled.
        /// </summary>
        [XmlElement("BuildingCollision")]
        public bool XMLBuildingCollision { get => HeightOffset.buildingCollision; set => HeightOffset.buildingCollision = value; }


        /// <summary>
        /// Network collision enabled/disabled.
        /// </summary>
        [XmlElement("NetworkCollision")]
        public bool XMLNetworkCollision { get => HeightOffset.networkCollision; set => HeightOffset.networkCollision = value; }


        /// <summary>
        /// Prop collision enabled/disabled.
        /// </summary>
        [XmlElement("PropCollision")]
        public bool XMLPropCollision { get => HeightOffset.propCollision; set => HeightOffset.propCollision = value; }


        /// <summary>
        /// Tree collision enabled/disabled.
        /// </summary>
        [XmlElement("TreeCollision")]
        public bool XMLTreeCollision { get => HeightOffset.treeCollision; set => HeightOffset.treeCollision = value; }


        /// <summary>
        /// Water bobbing enabled/disabled.
        /// </summary>
        [XmlElement("WaterBobbing")]
        public bool XMLWaterBobbing { get => HeightOffset.waterBobbing; set => HeightOffset.waterBobbing = value; }


        /// <summary>
        /// Minimum ground clearance.
        /// </summary>
        [XmlElement("GroundClearance")]
        public float XMLGroundClearance { get => HeightOffset.TerrainClearance; set => HeightOffset.TerrainClearance = value; }


        /// <summary>
        /// Follow disasters enabled/disabled.
        /// </summary>
        [XmlElement("FollowDisasters")]
        public bool XMLFollowDisasters { get => FollowDisasterPatch.followDisasters; set => FollowDisasterPatch.followDisasters = value; }


        /// <summary>
        /// Near clip distance.
        /// </summary>
        [XmlElement("NearClip")]
        public float XMLNearClip { get => CameraUtils.NearClipPlane; set => CameraUtils.NearClipPlane = value; }


        /// <summary>
        /// Speed multiplier.
        /// </summary>
        [XmlElement("SpeedMultiplier")]
        public int XMLSpeedMultiplier { get => (int)UpdateTargetPosition.CameraSpeed; set => UpdateTargetPosition.CameraSpeed = value; }


        /// <summary>
        /// Zoom to mouse cursor enabled/disabled.
        /// </summary>
        [XmlElement("ZoomToCursor")]
        public bool XMLZoomToCursor { get => ZoomToCursor; set => ZoomToCursor = value; }


        /// <summary>
        /// Disable follow rotation enabled/disabled.
        /// </summary>
        [XmlElement("DisableFollowRotation")]
        public bool XMLDisableFollowRotation { get => DisableFollowRotation; set => DisableFollowRotation = value; }


        // Hotkey element.
        [XmlElement("ToggleKey")]
        public KeyBinding ToggleKey
        {
            get => UUI.uuiKey.KeyBinding;

            set => UUI.uuiKey.KeyBinding = value;
        }


        // MoveIt zoom hotkey.
        [XmlElement("MoveItZoomKey")]
        public KeyBinding MoveItZoomKey
        {
            get => UIThreading.moveItKey;

            set => UIThreading.moveItKey = value;
        }


        // FPS mode hotkey.
        [XmlElement("FPSModeKey")]
        public KeyBinding FPSModeKey
        {
            get => UIThreading.fpsKey;

            set => UIThreading.fpsKey = value;
        }

        /// <summary>
        /// FPS key turning speed.
        /// </summary>
        [XmlElement("FPSKeyTurnSpeed")]
        public float XMLFPSKeyTurnSpeed { get => FPSPatch.KeyTurnSpeed; set => FPSPatch.KeyTurnSpeed = value; }


        /// <summary>
        /// FPS key movement speed.
        /// </summary>
        [XmlElement("FPSKeyMoveSpeed")]
        public float XMLFPSKeyMoveSpeed { get => FPSPatch.KeyMoveSpeed; set => FPSPatch.KeyMoveSpeed = value; }

        /// <summary>
        /// FPS mouse turning speed.
        /// </summary>
        [XmlElement("FPSMouseTurnSpeed")]
        public float XMLFPSMouseTurnSpeed { get => FPSPatch.MouseTurnSpeed; set => FPSPatch.MouseTurnSpeed = value; }


        // Mouse drag hotkey.
        [XmlElement("MapDragKey")]
        public KeyBinding MapDragKey
        {
            get => mapDragKey;

            set => mapDragKey = value;
        }


        /// <summary>
        /// Map dragging invert X axis.
        /// </summary>
        [XmlElement("MapDragInvertY")]
        public bool XMLMapDragInvertX { get => MapDragging.InvertXDrag; set => MapDragging.InvertXDrag = value; }


        /// <summary>
        /// Map dragging invert Y axis.
        /// </summary>
        [XmlElement("MapDragInvertX")]
        public bool XMLMapDragInvertY { get => MapDragging.InvertYDrag; set => MapDragging.InvertYDrag = value; }


        /// <summary>
        /// Map dragging speed.
        /// </summary>
        [XmlElement("MapDragSpeed")]
        public float XMLMapDragSpeed { get => MapDragging.DragSpeed; set => MapDragging.DragSpeed = value; }


        /// <summary>
        /// Zoom to cursor enabled.
        /// </summary>
        [XmlIgnore]
        internal static bool ZoomToCursor
        {
            get => zoomToCursor;

            set
            {
                zoomToCursor = value;

                // Apply/unapply Harmony patch on value change.
                Patcher.PatchZoomToCursor(value);
            }
        }
        [XmlIgnore]
        private static bool zoomToCursor = false;

        /// <summary>
        /// Disable rotation when following a target.
        /// </summary>
        [XmlIgnore]
        internal static bool DisableFollowRotation
        {
            get => disableFollowRotation;

            set
            {
                disableFollowRotation = value;

                // Apply/unapply Harmony patch on value change.
                Patcher.PatchFollowRotation(value);
            }
        }
        [XmlIgnore]
        private static bool disableFollowRotation = false;


        [XmlIgnore]
        internal static KeyBinding mapDragKey = new KeyBinding(KeyCode.Mouse1, false, true, false);


        /// <summary>
        /// Load settings from XML file.
        /// </summary>
        internal static void Load()
        {
            try
            {
                // Check to see if configuration file exists.
                if (File.Exists(SettingsFileName))
                {
                    // Read it.
                    using (StreamReader reader = new StreamReader(SettingsFileName))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                        if (!(xmlSerializer.Deserialize(reader) is ModSettings settingsFile))
                        {
                            Logging.Error("couldn't deserialize settings file");
                        }
                    }
                }
                else
                {
                    Logging.Message("no settings file found");
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception reading XML settings file");
            }
        }


        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        internal static void Save()
        {
            try
            {
                // Save into user local settings.
                using (StreamWriter writer = new StreamWriter(SettingsFileName))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                    xmlSerializer.Serialize(writer, new ModSettings());
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception saving XML settings file");
            }
        }
    }
}