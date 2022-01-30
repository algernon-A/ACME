using ICities;


namespace ACME
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            Logging.Message("loading");

            base.OnLevelLoaded(mode);

            // Apply camera settings.
            CameraUtils.ApplySettings();

            // Set up options panel event handler (need to redo this now that options panel has been reset after loading into game).
            OptionsPanelManager.OptionsEventHook();

            // Add UUI button.
            UUI.Setup();

            // Perform MoveIt reflection.
            ModUtils.MoveItReflection();

            // Activate keys.
            UIThreading.Operating = true;

            Logging.Message("loading complete");
        }
    }
}