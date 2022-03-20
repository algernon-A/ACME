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
        /// Toggle hotkey as ColossalFramework SavedInputKey.
        /// </summary>
        [XmlIgnore]
        internal static SavedInputKey ToggleSavedKey => UUI.uuiSavedKey;


        /// <summary>
        /// The current hotkey settings as ColossalFramework InputKey.
        /// </summary>
        [XmlIgnore]
        internal static InputKey CurrentHotkey
        {
            get => UUI.uuiSavedKey.value;

            set => UUI.uuiSavedKey.value = value;
        }


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


        // Hotkey element.
        [XmlElement("ToggleKey")]
        public KeyBinding ToggleKey
        {
            get
            {
                return new KeyBinding
                {
                    keyCode = (int)ToggleSavedKey.Key,
                    control = ToggleSavedKey.Control,
                    shift = ToggleSavedKey.Shift,
                    alt = ToggleSavedKey.Alt
                };
            }
            set
            {
                UUI.uuiSavedKey.Key = (UnityEngine.KeyCode)value.keyCode;
                UUI.uuiSavedKey.Control = value.control;
                UUI.uuiSavedKey.Shift = value.shift;
                UUI.uuiSavedKey.Alt = value.alt;
            }
        }


        // MoveIt zoom hotkey.
        [XmlElement("MoveItZoomKey")]
        public KeyBinding MoveItZoomKey
        {
            get
            {
                return new KeyBinding
                {
                    keyCode = (int)UIThreading.moveItKey,
                    control = UIThreading.moveItCtrl,
                    shift = UIThreading.moveItShift,
                    alt = UIThreading.moveItAlt
                };
            }
            set
            {
                // Backwads compatibility - this won't exist in older-format configuration files.
                if (value != null)
                {
                    UIThreading.moveItKey = (KeyCode)value.keyCode;
                    UIThreading.moveItCtrl = value.control;
                    UIThreading.moveItShift = value.shift;
                    UIThreading.moveItAlt = value.alt;
                }
            }
        }


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


    /// <summary>
    /// Basic keybinding class - code and modifiers.
    /// </summary>
    public class KeyBinding
    {
        [XmlAttribute("KeyCode")]
        public int keyCode;

        [XmlAttribute("Control")]
        public bool control;

        [XmlAttribute("Shift")]
        public bool shift;

        [XmlAttribute("Alt")]
        public bool alt;
    }
}