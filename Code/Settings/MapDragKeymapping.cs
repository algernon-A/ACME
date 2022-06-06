using ColossalFramework;


namespace ACME
{
    /// <summary>
    /// Keycode setting control for snap to MoveIt selection.
    /// </summary>
    public class MapDragKeymapping : OptionsKeymapping
    {
        /// <summary>
        /// Link to game hotkey setting.
        /// </summary>
        protected override InputKey KeySetting
        {
            get => ModSettings.mapDragKey.Encode();

            set
            {
                ModSettings.mapDragKey.keyCode = value & 0xFFFFFFF;
                ModSettings.mapDragKey.control = (value & 0x40000000) != 0;
                ModSettings.mapDragKey.shift = (value & 0x20000000) != 0;
                ModSettings.mapDragKey.alt = (value & 0x10000000) != 0;
            }
        }


        /// <summary>
        /// Control label.
        /// </summary>
        protected override string Label => Translations.Translate("KEY_MDG");
    }
}