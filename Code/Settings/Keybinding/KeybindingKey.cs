using UnityEngine;
using ColossalFramework;


namespace ACME
{

    /// <summary>
    /// Basic keybinding class - code and modifiers.
    /// </summary>
    public class KeybindingKey : KeybindingBase
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        public KeybindingKey()
        { }


        /// <summary>
        /// Constructor - assigns initial value.
        /// </summary>
        /// <param name="keyCode">Key code</param>
        public KeybindingKey(KeyCode keyCode)
        {
            key = keyCode;
        }


        /// <summary>
        /// Encode keybinding as saved input key for UUI.
        /// </summary>
        /// <returns></returns>
        public override InputKey Encode() => SavedInputKey.Encode(key, false, false, false);


        /// <summary>
        /// Sets the keybinding from the provided ColossalFramework InputKey.
        /// </summary>
        /// <param name="inputKey">InputKey to set from</param>
        public override void SetKey(InputKey inputKey)
        {
            key = (KeyCode)(inputKey & 0xFFFFFFF);
        }
    }
}
