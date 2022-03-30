using ICities;
using ACME.MessageBox;


namespace ACME
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        // Internal flags.
        internal static bool isLoaded = false;
        private static bool isModEnabled = false;
        private bool harmonyLoaded = false;

        // Used to flag if conflicting mods are running.
        private static bool conflictingMod = false;

        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            // Ensure that Harmony patches have been applied.
            harmonyLoaded = Patcher.Patched;
            if (!harmonyLoaded)
            {
                isModEnabled = false;
                Logging.Error("Harmony patches not applied; aborting");
                return;
            }

            // Check for mod conflicts.
            if (ModUtils.IsModConflict())
            {
                // Conflict detected.
                conflictingMod = true;
                isModEnabled = false;

                // Unload Harmony patches and exit before doing anything further.
                Patcher.UnpatchAll();
                return;
            }

            // Passed all checks - okay to load (if we haven't already fo some reason).
            if (!isModEnabled)
            {
                isModEnabled = true;
                Logging.KeyMessage("v " + ACMEMod.Version + " loading");
            }
        }


        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            // Check to see that Harmony 2 was properly loaded.
            if (!harmonyLoaded)
            {
                // Harmony 2 wasn't loaded; display warning notification and exit.
                ListMessageBox harmonyBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                harmonyBox.AddParas(Translations.Translate("ERR_HAR0"), Translations.Translate("CAM_ERR_HAR"), Translations.Translate("CAM_ERR_FAT"), Translations.Translate("ERR_HAR1"));

                // List of dot points.
                harmonyBox.AddList(Translations.Translate("ERR_HAR2"), Translations.Translate("ERR_HAR3"));

                // Closing para.
                harmonyBox.AddParas(Translations.Translate("MES_PAGE"));

                // Exit.
                return;
            }

            // Check to see if a conflicting mod has been detected.
            if (conflictingMod)
            {
                // Mod conflict detected - display warning notification and exit.
                ListMessageBox modConflictBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                modConflictBox.AddParas(Translations.Translate("ERR_CON0"), Translations.Translate("CAM_ERR_FAT"), Translations.Translate("CAM_ERR_CON0"), Translations.Translate("ERR_CON1"));

                // Add conflicting mod name(s).
                modConflictBox.AddList(ModUtils.conflictingModNames.ToArray());

                // Closing para.
                modConflictBox.AddParas(Translations.Translate("CAM_ERR_CON1"));

                // Exit.
                return;
            }

            base.OnLevelLoaded(mode);

            // Load mod if it's enabled.
            if (isModEnabled)
            {
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
}