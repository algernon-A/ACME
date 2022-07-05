using System.Xml.Serialization;
using UnityEngine;
using ColossalFramework;


namespace ACME
{

    /// <summary>
    /// Basic keybinding class - code and modifiers.
    /// </summary>
    public class KeyBinding
    {
        [XmlIgnore]
        public KeyCode key;

        [XmlAttribute("Control")]
        public bool control;

        [XmlAttribute("Shift")]
        public bool shift;

        [XmlAttribute("Alt")]
        public bool alt;

        [XmlAttribute("KeyCode")]
        public int Key
        {
            get => (int)key;

            set => key = (KeyCode)value;
        }


        /// <summary>
        /// Basic constructor.
        /// </summary>
        public KeyBinding()
        { }


        /// <summary>
        /// Constructor - assigns initial values.
        /// </summary>
        /// <param name="keyCode">Key code</param>
        /// <param name="control">Control modifier key status</param>
        /// <param name="shift">Shift modifier key status</param>
        /// <param name="alt">Alt modifier key status</param>
        public KeyBinding(KeyCode keyCode, bool control, bool shift, bool alt)
        {
            this.key = keyCode;
            this.control = control;
            this.shift = shift;
            this.alt = alt;
        }


        /// <summary>
        /// Encode keybinding as saved input key for UUI.
        /// </summary>
        /// <returns></returns>
        public InputKey Encode() => SavedInputKey.Encode(key, control, shift, alt);


        /// <summary>
        /// Checks to see if the designated key is pressed.
        /// </summary>
        /// <returns>True if pressed, false otherwise</returns>
        public bool IsPressed()
        {
            // Check primary key.
            if (!Input.GetKey((KeyCode)key))
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
        /// Current value as KeyBinding.
        /// </summary>
        public KeyBinding KeyBinding
        {
            get => new KeyBinding(Key, Control, Shift, Alt);
            set => this.value = value.Encode();
        }
    }
}
