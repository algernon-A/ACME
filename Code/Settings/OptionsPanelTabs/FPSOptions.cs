// <copyright file="FPSOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons.Keybinding;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework;
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

        // FPS keybindings.
        private readonly KeyOnlyBinding[] keyBindings = new KeyOnlyBinding[(int)KeyBindingIndex.NumKeys]
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
        private readonly string[] keyLabels = new string[(int)KeyBindingIndex.NumKeys]
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

        // Keymapping controls.
        private readonly OptionsKeymapping[] keyMappingControls = new OptionsKeymapping[(int)KeyBindingIndex.NumKeys];

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
            currentY += fpsKeyMapping.Panel.height + Margin;

            // Key turning speed slider.
            UISlider keyTurnSlider = UISliders.AddPlainSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_FPS_KTS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.KeyTurnSpeed);
            keyTurnSlider.eventValueChanged += (c, value) => { FPSPatch.KeyTurnSpeed = value; };
            currentY += keyTurnSlider.parent.height + Margin;

            // Key movement speed slider.
            UISlider keyMoveSlider = UISliders.AddPlainSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_FPS_KMS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.KeyMoveSpeed);
            keyMoveSlider.eventValueChanged += (c, value) => { FPSPatch.KeyMoveSpeed = value; };
            currentY += keyMoveSlider.parent.height + Margin;

            // Mouse turning speed slider.
            UISlider mouseTurnSlider = UISliders.AddPlainSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_FPS_MTS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.MouseTurnSpeed);
            mouseTurnSlider.eventValueChanged += (c, value) => { FPSPatch.MouseTurnSpeed = value; };
            currentY += mouseTurnSlider.parent.height + Margin;

            // Add scrollable panel for hotkeys.
            UIScrollablePanel scrollPanel = panel.AddUIComponent<UIScrollablePanel>();
            scrollPanel.relativePosition = new Vector2(0, currentY);
            scrollPanel.autoSize = false;
            scrollPanel.autoLayout = false;
            scrollPanel.width = panel.width - GroupMargin;
            scrollPanel.height = panel.height - currentY - GroupMargin;
            scrollPanel.clipChildren = true;
            scrollPanel.builtinKeyNavigation = true;
            scrollPanel.scrollWheelDirection = UIOrientation.Vertical;
            UIScrollbars.AddScrollbar(panel, scrollPanel);

            // Add fps keys.
            float currentKeyY = 0;
            for (int i = 0; i < (int)KeyBindingIndex.NumKeys; ++i)
            {
                keyMappingControls[i] = scrollPanel.gameObject.AddComponent<OptionsKeymapping>();
                keyMappingControls[i].Label = Translations.Translate(keyLabels[i]);
                keyMappingControls[i].Binding = keyBindings[i];
                keyMappingControls[i].Panel.relativePosition = new Vector2(LeftMargin, currentKeyY);
                currentKeyY += keyMappingControls[i].Panel.height;
            }

            UIButton resetButton = UIButtons.AddButton(panel, 0f, scrollPanel.relativePosition.y + scrollPanel.height + Margin, Translations.Translate("KEY_FPS_RES"), width: 300f);
            resetButton.tooltip = Translations.Translate("KEY_FPS_RES_TIP");
            resetButton.eventClicked += (c, p) =>
            {
                // Sync to game.
                keyMappingControls[(int)KeyBindingIndex.CameraMoveForward].KeySetting = new SavedInputKey(Settings.cameraMoveForward, Settings.gameSettingsFile, DefaultSettings.cameraMoveForward, autoUpdate: false);
                keyMappingControls[(int)KeyBindingIndex.CameraMoveBackward].KeySetting = new SavedInputKey(Settings.cameraMoveBackward, Settings.gameSettingsFile, DefaultSettings.cameraMoveBackward, autoUpdate: false);
                keyMappingControls[(int)KeyBindingIndex.CameraMoveLeft].KeySetting = new SavedInputKey(Settings.cameraMoveLeft, Settings.gameSettingsFile, DefaultSettings.cameraMoveLeft, autoUpdate: false);
                keyMappingControls[(int)KeyBindingIndex.CameraMoveRight].KeySetting = new SavedInputKey(Settings.cameraMoveRight, Settings.gameSettingsFile, DefaultSettings.cameraMoveRight, autoUpdate: false);
                keyMappingControls[(int)KeyBindingIndex.CameraRotateLeft].KeySetting = new SavedInputKey(Settings.cameraRotateLeft, Settings.gameSettingsFile, DefaultSettings.cameraRotateLeft, autoUpdate: false);
                keyMappingControls[(int)KeyBindingIndex.CameraRotateRight].KeySetting = new SavedInputKey(Settings.cameraRotateRight, Settings.gameSettingsFile, DefaultSettings.cameraRotateRight, autoUpdate: false);
                keyMappingControls[(int)KeyBindingIndex.CameraRotateUp].KeySetting = new SavedInputKey(Settings.cameraRotateUp, Settings.gameSettingsFile, DefaultSettings.cameraRotateUp, autoUpdate: false);
                keyMappingControls[(int)KeyBindingIndex.CameraRotateDown].KeySetting = new SavedInputKey(Settings.cameraRotateDown, Settings.gameSettingsFile, DefaultSettings.cameraRotateDown, autoUpdate: false);
                keyMappingControls[(int)KeyBindingIndex.CameraMouseRotate].KeySetting = new SavedInputKey(Settings.cameraMouseRotate, Settings.gameSettingsFile, DefaultSettings.cameraMouseRotate, autoUpdate: false);

                // Reset mod defaults (no game equivalent).
                keyMappingControls[(int)KeyBindingIndex.AbsUp].KeySetting = new KeyOnlyBinding(KeyCode.PageUp).Encode();
                keyMappingControls[(int)KeyBindingIndex.AbsDown].KeySetting = new KeyOnlyBinding(KeyCode.PageDown).Encode();
                keyMappingControls[(int)KeyBindingIndex.AbsLeft].KeySetting = new KeyOnlyBinding(KeyCode.LeftArrow).Encode();
                keyMappingControls[(int)KeyBindingIndex.AbsRight].KeySetting = new KeyOnlyBinding(KeyCode.RightArrow).Encode();
                keyMappingControls[(int)KeyBindingIndex.AbsForward].KeySetting = new KeyOnlyBinding(KeyCode.UpArrow).Encode();
                keyMappingControls[(int)KeyBindingIndex.AbsBack].KeySetting = new KeyOnlyBinding(KeyCode.DownArrow).Encode();
            };
        }

        /// <summary>
        /// Keybinding index enum.
        /// </summary>
        private enum KeyBindingIndex : int
        {
            CameraMoveForward = 0,
            CameraMoveBackward,
            CameraMoveLeft,
            CameraMoveRight,
            AbsForward,
            AbsBack,
            AbsLeft,
            AbsRight,
            AbsUp,
            AbsDown,
            CameraRotateUp,
            CameraRotateDown,
            CameraRotateLeft,
            CameraRotateRight,
            CameraMouseRotate,
            NumKeys,
        }
    }
}