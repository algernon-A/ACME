// <copyright file="FPSOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons.Keybinding;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Options panel for setting FPS options.
    /// </summary>
    internal class FPSOptions
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;

        // Number of keybindings.
        private const int NumKeys = 15;

        // FPS keybindings.
        private KeyOnlyBinding[] keyBindings = new KeyOnlyBinding[NumKeys]
        {
            FPSPatch.CameraMoveForward,
            FPSPatch.CameraMoveBackward,
            FPSPatch.CameraMoveLeft,
            FPSPatch.CameraMoveRight,
            FPSPatch.AbsForward,
            FPSPatch.AbsBack,
            FPSPatch.AbsLeft,
            FPSPatch.AbsRight,
            FPSPatch.AbsUp,
            FPSPatch.AbsDown,
            FPSPatch.CameraRotateUp,
            FPSPatch.CameraRotateDown,
            FPSPatch.CameraRotateLeft,
            FPSPatch.CameraRotateRight,
            FPSPatch.CameraMouseRotate,
        };

        // FPS keybinding labels.
        private string[] keyLabels = new string[NumKeys]
        {
            "KEY_REL_FWD",
            "KEY_REL_BCK",
            "KEY_REL_LFT",
            "KEY_REL_RHT",
            "KEY_ABS_FWD",
            "KEY_ABS_BCK",
            "KEY_ABS_LFT",
            "KEY_ABS_RHT",
            "KEY_ABS_UP",
            "KEY_ABS_DWN",
            "KEY_ROT_UP",
            "KEY_ROT_DWN",
            "KEY_ROT_LFT",
            "KEY_ROT_RHT",
            "KEY_ROT_MSE",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FPSOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal FPSOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("CAM_OPT_FPS"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = GroupMargin;

            // FPS mode key control.
            OptionsKeymapping fpsKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            fpsKeyMapping.Label = Translations.Translate("KEY_FPS");
            fpsKeyMapping.Binding = UIThreading.FPSModeKey;
            fpsKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += fpsKeyMapping.Panel.height + GroupMargin;

            // Key turning speed slider.
            UISlider keyTurnSlider = UISliders.AddPlainSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_FPS_KTS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.KeyTurnSpeed);
            keyTurnSlider.eventValueChanged += (c, value) => { FPSPatch.KeyTurnSpeed = value; };
            currentY += keyTurnSlider.parent.height + GroupMargin;

            // Key movement speed slider.
            UISlider keyMoveSlider = UISliders.AddPlainSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_FPS_KMS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.KeyMoveSpeed);
            keyMoveSlider.eventValueChanged += (c, value) => { FPSPatch.KeyMoveSpeed = value; };
            currentY += keyMoveSlider.parent.height + GroupMargin;

            // Mouse turning speed slider.
            UISlider mouseTurnSlider = UISliders.AddPlainSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_FPS_MTS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.MouseTurnSpeed);
            mouseTurnSlider.eventValueChanged += (c, value) => { FPSPatch.MouseTurnSpeed = value; };
            currentY += mouseTurnSlider.parent.height + GroupMargin;

            // Add fps keys.
            for (int i = 0; i < NumKeys; ++i)
            {
                OptionsKeymapping keyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
                keyMapping.Label = Translations.Translate(keyLabels[i]);
                keyMapping.Binding = keyBindings[i];
                keyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
                currentY += keyMapping.Panel.height;
            }

            // Set panel height.
            panel.height = currentY;
            PanelHeight = currentY;
        }

        /// <summary>
        /// Gets or sets the panel height.
        /// </summary>
        internal float PanelHeight { get; set; }
    }
}