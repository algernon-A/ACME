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


        // FPS mode hotkey.
        [XmlElement("FPSModeKey")]
        public KeyBinding FPSModeKey
        {
            get
            {
                return new KeyBinding
                {
                    keyCode = (int)UIThreading.fpsKey,
                    control = UIThreading.fpsCtrl,
                    shift = UIThreading.fpsShift,
                    alt = UIThreading.fpsAlt
                };
            }
            set
            {
                // Backwads compatibility - this won't exist in older-format configuration files.
                if (value != null)
                {
                    UIThreading.fpsKey = (KeyCode)value.keyCode;
                    UIThreading.fpsCtrl = value.control;
                    UIThreading.fpsShift = value.shift;
                    UIThreading.fpsAlt = value.alt;
                }
            }
        }


        // Mouse drag hotkey.
        [XmlElement("MapDragKey")]
        public KeyBinding MapDragKey
        {
            get => mapDragKey;

            set => mapDragKey = value;
        }


        [XmlIgnore]
        internal static KeyBinding mapDragKey = new KeyBinding
        {
            keyCode = (int)KeyCode.Mouse1,
            control = false,
            shift = true,
            alt = false
        };


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


        /// <summary>
        /// Encode keybinding as saved input key for UUI.
        /// </summary>
        /// <returns></returns>
        public InputKey Encode() => SavedInputKey.Encode((KeyCode)keyCode, control, shift, alt);


        /// <summary>
        /// Checks to see if the designated key is pressed.
        /// </summary>
        /// <returns>True if pressed, false otherwise</returns>
        public bool IsPressed()
        {
            // Check primary key.
            if (!Input.GetKey((KeyCode)keyCode))
            {
                return false;
            }

            // Check modifier keys,
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) != control)
            {
                return false;
            }
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) != shift)
            {
                return false;
            }
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr)) != alt)
            {
                return false;
            }

            // If we got here, all checks have been passed.
            return true;
        }
    }


    /// <summary>
    /// UUI unsaved input key.
    /// </summary>
    public class UnsavedInputKey : UnifiedUI.Helpers.UnsavedInputKey
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Reference name</param>
        /// <param name="keyCode">Keycode</param>
        /// <param name="control">Control modifier key status</param>
        /// <param name="shift">Shift modifier key status</param>
        /// <param name="alt">Alt modifier key status</param>
        public UnsavedInputKey(string name, KeyCode keyCode, bool control, bool shift, bool alt) :
            base(keyName: name, modName: "ACME", Encode(keyCode, control: control, shift: shift, alt: alt))
        {
        }


        /// <summary>
        /// Called by UUI when a key conflict is resolved.
        /// Used here to save the new key setting.
        /// </summary>
        public override void OnConflictResolved() => ModSettings.Save();


        /// <summary>
        /// 
        /// </summary>
        public KeyBinding KeyBinding
        {
            get => new KeyBinding { keyCode = (int)Key, control = Control, shift = Shift, alt = Alt };
            set => this.value = value.Encode();
        }
    }
}