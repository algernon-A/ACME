// <copyright file="UUI.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons.Translation;
    using UnifiedUI.Helpers;
    using UnityEngine;

    /// <summary>
    /// Static class to handle UUI interface.
    /// </summary>
    internal static class UUI
    {
        // UUI Button.
        private static UUICustomButton s_uuiButton;

        /// <summary>
        /// Gets or sets the UnsavedInputKey reference for communicating with UUI.
        /// </summary>
        internal static UnsavedInputKey UUIKey { get; set; } = new UnsavedInputKey("ACME hotkey", keyCode: KeyCode.C, control: false, shift: false, alt: true);

        /// <summary>
        /// Gets the UUI button instance.
        /// </summary>
        internal static UUICustomButton UUIButton => s_uuiButton;

        /// <summary>
        /// Performs initial setup and creates the UUI button.
        /// </summary>
        internal static void Setup()
        {
            // Add UUI button.
            if (s_uuiButton == null)
            {
                s_uuiButton = UUIHelpers.RegisterCustomButton(
                    name: Mod.Instance.BaseName,
                    groupName: null, // default group
                    tooltip: Translations.Translate("MOD_NAME"),
                    icon: UUIHelpers.LoadTexture(UUIHelpers.GetFullPath<Mod>("Resources", "ACME-UUI.png")),
                    onToggle: (value) => CameraPanel.SetState(value),
                    hotkeys: new UUIHotKeys { ActivationKey = UUIKey });

                // Set initial state.
                s_uuiButton.IsPressed = false;
            }
        }
    }
}