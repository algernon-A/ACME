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


        /// <summary>
        /// Adds mod options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
        internal FPSOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = PanelUtils.AddTab(tabStrip, Translations.Translate("CAM_OPT_FPS"), tabIndex, false);

            // Add controls.

            // Y position indicator.
            float currentY = GroupMargin;

            // FPS mode key control.
            OptionsKeymapping fpsKeyMapping = panel.gameObject.AddComponent<FPSKeymapping>();
            fpsKeyMapping.uIPanel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += fpsKeyMapping.uIPanel.height + GroupMargin;

            // Key turning speed slider.
            UISlider keyTurnSlider = UIControls.AddSliderWithValue(panel, Translations.Translate("CAM_FPS_KTS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.KeyTurnSpeed);
            keyTurnSlider.parent.relativePosition = new Vector2(Margin, currentY);
            keyTurnSlider.eventValueChanged += (control, value) => { FPSPatch.KeyTurnSpeed = value; };
            currentY += keyTurnSlider.parent.height + GroupMargin;

            // Key movement speed slider.
            UISlider keyMoveSlider = UIControls.AddSliderWithValue(panel, Translations.Translate("CAM_FPS_KMS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.KeyMoveSpeed);
            keyMoveSlider.parent.relativePosition = new Vector2(Margin, currentY);
            keyMoveSlider.eventValueChanged += (control, value) => { FPSPatch.KeyMoveSpeed = value; };
            currentY += keyMoveSlider.parent.height + GroupMargin;

            // Mouse turning speed slider.
            UISlider mouseTurnSlider = UIControls.AddSliderWithValue(panel, Translations.Translate("CAM_FPS_MTS"), FPSPatch.MinFPSKeySpeed, FPSPatch.MaxFPSKeySpeed, 0.1f, FPSPatch.MouseTurnSpeed);
            mouseTurnSlider.parent.relativePosition = new Vector2(Margin, currentY);
            mouseTurnSlider.eventValueChanged += (control, value) => { FPSPatch.MouseTurnSpeed = value; };
        }
    }
}