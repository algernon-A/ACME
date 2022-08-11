// <copyright file="Loading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons;
    using AlgernonCommons.Notifications;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ICities;

    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        // Internal flags.
        private bool _isModEnabled = false;
        private bool _conflictingMod = false;
        private bool _harmonyLoaded = false;

        /// <summary>
        /// Gets a value indicating whether the mod has finished loading.
        /// </summary>
        internal static bool IsLoaded { get; private set; } = false;

        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game, editor, scenario, etc.).</param>
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            // Ensure that Harmony patches have been applied.
            _harmonyLoaded = Patcher.Instance.Patched;
            if (!_harmonyLoaded)
            {
                _isModEnabled = false;
                Logging.Error("Harmony patches not applied; aborting");
                return;
            }

            // Check for mod conflicts.
            if (ConflictDetection.IsModConflict())
            {
                // Conflict detected.
                _conflictingMod = true;
                _isModEnabled = false;

                // Unload Harmony patches and exit before doing anything further.
                Patcher.Instance.UnpatchAll();
                return;
            }

            // Passed all checks - okay to load (if we haven't already fo some reason).
            if (!_isModEnabled)
            {
                _isModEnabled = true;
                Logging.KeyMessage(Mod.Instance.Name, " loading");
            }
        }

        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.).</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            // Check to see that Harmony 2 was properly loaded.
            if (!_harmonyLoaded)
            {
                // Harmony 2 wasn't loaded; display warning notification and exit.
                ListNotification harmonyBox = NotificationBase.ShowNotification<ListNotification>();

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
            if (_conflictingMod)
            {
                // Mod conflict detected - display warning notification and exit.
                ListNotification modConflictBox = NotificationBase.ShowNotification<ListNotification>();

                // Key text items.
                modConflictBox.AddParas(Translations.Translate("ERR_CON0"), Translations.Translate("CAM_ERR_FAT"), Translations.Translate("CAM_ERR_CON0"), Translations.Translate("ERR_CON1"));

                // Add conflicting mod name(s).
                modConflictBox.AddList(ConflictDetection.ConflictingModNames.ToArray());

                // Closing para.
                modConflictBox.AddParas(Translations.Translate("CAM_ERR_CON1"));

                // Exit.
                return;
            }

            base.OnLevelLoaded(mode);

            // Load mod if it's enabled.
            if (_isModEnabled)
            {
                // Apply camera settings.
                CameraUtils.ApplySettings();

                // Set up options panel event handler (need to redo this now that options panel has been reset after loading into game).
                OptionsPanelManager<OptionsPanel>.OptionsEventHook();

                // Add UUI button.
                UUI.Setup();

                // Perform MoveIt reflection.
                MoveItUtils.MoveItReflection();

                // Activate keys.
                UIThreading.Operating = true;

                Logging.Message("loading complete");
            }
        }
    }
}