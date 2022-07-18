using System.Xml.Serialization;
using UnityEngine;
using ColossalFramework;


namespace ACME
{

    /// <summary>
    /// Basic keybinding class - code and modifiers.
    /// </summary>
    public class KeyBinding : KeybindingBase
    {
        [XmlAttribute("Control")]
        public bool control;

        [XmlAttribute("Shift")]
        public bool shift;

        [XmlAttribute("Alt")]
        public bool alt;


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
        public override InputKey Encode() => SavedInputKey.Encode(key, control, shift, alt);


        /// <summary>
        /// Sets the keybinding from the provided ColossalFramework InputKey.
        /// </summary>
        /// <param name="inputKey">InputKey to set from</param>
        public override void SetKey(InputKey inputKey)
        {
            key = (KeyCode)(inputKey & 0xFFFFFFF);
            control = (inputKey & 0x40000000) != 0;
            shift = (inputKey & 0x20000000) != 0;
            alt = (inputKey & 0x10000000) != 0;
        }


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
}