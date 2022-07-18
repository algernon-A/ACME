using UnityEngine;
using ColossalFramework.UI;


namespace ACME
{
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


        // Panel height calculation.
        internal float panelHeight;


        // FPS keybindings.
        private KeybindingKey[] keyBindings = new KeybindingKey[NumKeys]
        {
            FPSPatch.cameraMoveForward,
            FPSPatch.cameraMoveBackward,
            FPSPatch.cameraMoveLeft,
            FPSPatch.cameraMoveRight,
            FPSPatch.absForward,
            FPSPatch.absBack,
            FPSPatch.absLeft,
            FPSPatch.absRight,
            FPSPatch.absUp,
            FPSPatch.absDown,
            FPSPatch.cameraRotateUp,
            FPSPatch.cameraRotateDown,
            FPSPatch.cameraRotateLeft,
            FPSPatch.cameraRotateRight,
            FPSPatch.cameraMouseRotate
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
            "KEY_ROT_MSE"
        };

        UIPanel panel;


        /// <summary>
        /// Adds mod options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
        internal FPSOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            panel = PanelUtils.AddTab(tabStrip, Translations.Translate("CAM_OPT_FPS"), tabIndex, false);

            // Y position indicator.
            float currentY = GroupMargin;

            // FPS mode key control.
            OptionsKeymapping fpsKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            fpsKeyMapping.Label = Translations.Translate("KEY_FPS");
            fpsKeyMapping.Binding = UIThreading.fpsKey;
            fpsKeyMapping.uIPanel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += fpsKeyMapping.uIPanel.height + GroupMargin;

            // Key turning speed slider.
            UISlider keyTurnSlider = UIControls.AddSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_FPS_KTS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.KeyTurnSpeed, (value) => { FPSPatch.KeyTurnSpeed = value; });
            currentY += keyTurnSlider.parent.height + GroupMargin;

            // Key movement speed slider.
            UISlider keyMoveSlider = UIControls.AddSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_FPS_KMS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.KeyMoveSpeed, (value) => { FPSPatch.KeyMoveSpeed = value; });
            currentY += keyMoveSlider.parent.height + GroupMargin;

            // Mouse turning speed slider.
            UISlider mouseTurnSlider = UIControls.AddSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_FPS_MTS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.MouseTurnSpeed, (value) => { FPSPatch.MouseTurnSpeed = value; });
            mouseTurnSlider.eventValueChanged += (control, value) => { FPSPatch.MouseTurnSpeed = value; };
            currentY += mouseTurnSlider.parent.height + GroupMargin;

            // Add fps keys.
            for (int i = 0; i < NumKeys; ++i)
            {
                OptionsKeymapping keyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
                keyMapping.Label = Translations.Translate(keyLabels[i]);
                keyMapping.Binding = keyBindings[i];
                keyMapping.uIPanel.relativePosition = new Vector2(LeftMargin, currentY);
                currentY += keyMapping.uIPanel.height;
            }

            // Set panel height.
            panel.height = currentY;
            panelHeight = currentY;
        }
    }
}