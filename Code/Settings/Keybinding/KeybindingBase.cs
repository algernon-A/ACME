using System.Xml.Serialization;
using UnityEngine;
using ColossalFramework;


namespace ACME
{
    /// <summary>
    /// Basic keybinding class - code and modifiers.
    /// </summary>
    public abstract class KeybindingBase
    {
        [XmlIgnore]
        public KeyCode key;


        [XmlAttribute("KeyCode")]
        public int Key
        {
            get => (int)key;

            set => key = (KeyCode)value;
        }


        /// <summary>
        /// Encode keybinding as saved input key for UUI.
        /// </summary>
        /// <returns></returns>
        public abstract InputKey Encode();


        /// <summary>
        /// Sets the keybinding from the provided ColossalFramework InputKey.
        /// </summary>
        /// <param name="inputKey">InputKey to set from</param>
        public abstract void SetKey(InputKey inputKey);
    }
}
