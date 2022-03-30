using UnityEngine;
using ColossalFramework;


namespace ACME
{
    /// <summary>
    /// Keycode setting control for snap to MoveIt selection.
    /// </summary>
    public class FPSKeymapping : OptionsKeymapping
    {
        /// <summary>
        /// Link to game hotkey setting.
        /// </summary>
        protected override InputKey KeySetting
        {
            get => SavedInputKey.Encode(UIThreading.fpsKey, UIThreading.fpsCtrl, UIThreading.fpsShift, UIThreading.fpsAlt);

            set
            {
                UIThreading.fpsKey = (KeyCode)(value & 0xFFFFFFF);
                UIThreading.fpsCtrl = (value & 0x40000000) != 0;
                UIThreading.fpsShift = (value & 0x20000000) != 0;
                UIThreading.fpsAlt = (value & 0x10000000) != 0;
            }
        }


        /// <summary>
        /// Control label.
        /// </summary>
        protected override string Label => Translations.Translate("KEY_FPS");
    }
}