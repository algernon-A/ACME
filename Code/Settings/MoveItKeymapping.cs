using UnityEngine;
using ColossalFramework;


namespace ACME
{
    /// <summary>
    /// Keycode setting control for snap to MoveIt selection.
    /// </summary>
    public class MoveItKeymapping : OptionsKeymapping
    {
        /// <summary>
        /// Link to game hotkey setting.
        /// </summary>
        protected override InputKey KeySetting
        {
            get => SavedInputKey.Encode(UIThreading.moveItKey, UIThreading.moveItCtrl, UIThreading.moveItShift, UIThreading.moveItAlt);

            set
            {
                UIThreading.moveItKey = (KeyCode)(value & 0xFFFFFFF);
                UIThreading.moveItCtrl = (value & 0x40000000) != 0;
                UIThreading.moveItShift = (value & 0x20000000) != 0;
                UIThreading.moveItAlt = (value & 0x10000000) != 0;
            }
        }


        /// <summary>
        /// Control label.
        /// </summary>
        protected override string Label => Translations.Translate("KEY_GMI");
    }
}