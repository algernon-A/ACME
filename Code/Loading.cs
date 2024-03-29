﻿// <copyright file="Loading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System.Collections.Generic;
    using AlgernonCommons.Patching;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ICities;

    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public sealed class Loading : PatcherLoadingBase<OptionsPanel, Patcher>
    {
        /// <summary>
        /// Gets any text for a trailing confict notification paragraph (e.g. "These mods must be removed before this mod can operate").
        /// </summary>
        protected override string ConflictRemovedText => Translations.Translate("CAM_ERR_CON1");

        /// <summary>
        /// Gets a list of permitted loading modes.
        /// </summary>
        protected override List<AppMode> PermittedModes => new List<AppMode> { AppMode.Game, AppMode.MapEditor, AppMode.AssetEditor, AppMode.ThemeEditor, AppMode.ScenarioEditor };

        /// <summary>
        /// Checks for any mod conflicts.
        /// Called as part of checking prior to executing any OnCreated actions.
        /// </summary>
        /// <returns>A list of conflicting mod names (null or empty if none).</returns>
        protected override List<string> CheckModConflicts() => ConflictDetection.CheckConflictingMods();

        /// <summary>
        /// Performs any actions upon successful level loading completion.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.).</param>
        protected override void LoadedActions(LoadMode mode)
        {
            // Apply camera settings.
            CameraUtils.ApplySettings();

            // Set up options panel event handler (need to redo this now that options panel has been reset after loading into game).
            OptionsPanelManager<OptionsPanel>.OptionsEventHook();

            // Add UUI button.
            UUI.Setup();

            // Perform MoveIt reflection.
            MoveItUtils.MoveItReflection();

            // Activate patches if required.
            PatcherManager<Patcher>.Instance.PatchFollowRotation(ModSettings.DisableFollowRotation);
            PatcherManager<Patcher>.Instance.PatchZoomToCursor(ModSettings.ZoomToCursor);

            // Activate keys.
            UIThreading.Operating = true;
        }
    }
}