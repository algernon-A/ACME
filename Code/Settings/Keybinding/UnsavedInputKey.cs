using UnityEngine;


namespace ACME
{
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
