using ColossalFramework;


namespace ACME
{
    /// <summary>
    /// Keycode setting control for UUI key.
    /// </summary>
    public class UUIKeymapping : OptionsKeymapping
    {
        /// <summary>
        /// Link to game hotkey setting.
        /// </summary>
        protected override InputKey KeySetting
        {
            get => UUI.uuiKey.value;

            set => UUI.uuiKey.value = value;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public UUIKeymapping()
        {
            // Set label and button text.
            Label = Translations.Translate("KEY_KEY");
            button.text = SavedInputKey.ToLocalizedString("KEYNAME", KeySetting);
        }
    }
}