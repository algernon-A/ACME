using UnityEngine;
using ColossalFramework;


namespace ACME
{
    /// <summary>
    /// Keycode setting control for snap to MoveIt selection.
    /// </summary>
    public class MouseDragKeymapping : OptionsKeymapping
    {
        /// <summary>
        /// Link to game hotkey setting.
        /// </summary>
        protected override InputKey KeySetting
        {
            get => ModSettings.mouseDragKey;

            set => ModSettings.mouseDragKey.value = value;
        }


        /// <summary>
        /// Control label.
        /// </summary>
        protected override string Label => Translations.Translate("KEY_MDG");
    }
}