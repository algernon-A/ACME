using UnityEngine;
using ColossalFramework.UI;


namespace ACME
{
    /// <summary>
    /// Options panel for setting basic mod options.
    /// </summary>
    internal class GeneralOptions
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
        internal GeneralOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = PanelUtils.AddTab(tabStrip, Translations.Translate("CAM_OPT_GEN"), tabIndex, false);

            // Add controls.

            // Y position indicator.
            float currentY = Margin;

            // Language choice.
            UIDropDown languageDropDown = UIControls.AddPlainDropDown(panel, Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanel.LocaleChanged();
            };
            languageDropDown.parent.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += languageDropDown.parent.height + Margin;

            // Hotkey control.
            OptionsKeymapping uuiKeyMapping = languageDropDown.parent.parent.gameObject.AddComponent<UUIKeymapping>();
            uuiKeyMapping.uIPanel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += uuiKeyMapping.uIPanel.height + Margin;

            // MoveIt key control.
            OptionsKeymapping miKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            miKeyMapping.Label = Translations.Translate("KEY_GMI");
            miKeyMapping.Binding = UIThreading.moveItKey;
            miKeyMapping.uIPanel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += miKeyMapping.uIPanel.height + Margin;

            // Building collision checkbox.
            UICheckBox buildingCollisionCheck = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_BLD"));
            buildingCollisionCheck.isChecked = HeightOffset.buildingCollision;
            buildingCollisionCheck.eventCheckChanged += (control, value) => { HeightOffset.buildingCollision = value; };
            currentY += buildingCollisionCheck.height + Margin;

            // Network collision checkbox.
            UICheckBox netCollisionCHeck = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_NET"));
            netCollisionCHeck.isChecked = HeightOffset.networkCollision;
            netCollisionCHeck.eventCheckChanged += (control, value) => { HeightOffset.networkCollision = value; };
            currentY += netCollisionCHeck.height + Margin;

            // Prop collision checkbox.
            UICheckBox propCollisionCHeck = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_PRO"));
            propCollisionCHeck.isChecked = HeightOffset.propCollision;
            propCollisionCHeck.eventCheckChanged += (control, value) => { HeightOffset.propCollision = value; };
            currentY += propCollisionCHeck.height + Margin;

            // Tree collision checkbox.
            UICheckBox treeCollisionCheck = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_TRE"));
            treeCollisionCheck.isChecked = HeightOffset.treeCollision;
            treeCollisionCheck.eventCheckChanged += (control, value) => { HeightOffset.treeCollision = value; };
            currentY += propCollisionCHeck.height + Margin;

            // Water bobbing checkbox.
            UICheckBox waterBobbingCheck = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_WAT"));
            waterBobbingCheck.isChecked = HeightOffset.waterBobbing;
            waterBobbingCheck.eventCheckChanged += (control, value) => { HeightOffset.waterBobbing = value; };
            currentY += propCollisionCHeck.height + GroupMargin;

            // Ground proximity slider .
            UISlider groundProximitySlider = AddDistanceSlider(panel, ref currentY, "CAM_COL_GND", HeightOffset.MinTerrainClearance, HeightOffset.MaxTerrainClearance, HeightOffset.TerrainClearance);
            groundProximitySlider.eventValueChanged += (control, value) => { HeightOffset.TerrainClearance = value; };

            // Near clipping slider.
            UISlider nearClipSlider = AddDistanceSlider(panel, ref currentY, "CAM_CLP_NEA", CameraUtils.MinNearClip, CameraUtils.MaxNearClip, CameraUtils.NearClipPlane);
            nearClipSlider.eventValueChanged += (control, value) => { CameraUtils.NearClipPlane = value; };

            // Speed multiplier.
            UISlider speedSlider = AddSlider(panel, ref currentY, "CAM_SPD_MIN", UpdateTargetPosition.MinCameraSpeed, UpdateTargetPosition.MaxCameraSpeed, UpdateTargetPosition.CameraSpeed);
            speedSlider.eventValueChanged += (control, value) => { UpdateTargetPosition.CameraSpeed = value; };
            currentY += Margin;

            // Follow disasters checkbox.
            UICheckBox disableDisasterGoto = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_OPT_DIS"));
            disableDisasterGoto.isChecked = FollowDisasterPatch.followDisasters;
            disableDisasterGoto.eventCheckChanged += (control, value) => { FollowDisasterPatch.followDisasters = value; };
            currentY += disableDisasterGoto.height + Margin;

            // Zoom to cursor.
            UICheckBox zoomToCursorCheck = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_OPT_ZTC"));
            zoomToCursorCheck.isChecked = ModSettings.ZoomToCursor;
            zoomToCursorCheck.eventCheckChanged += (control, value) => { ModSettings.ZoomToCursor = value; };
            zoomToCursorCheck.tooltipBox = TooltipUtils.TooltipBox;
            zoomToCursorCheck.tooltip = Translations.Translate("CAM_OPT_ZTC_TIP");
            currentY += zoomToCursorCheck.height + Margin;

            // Follow disasters checkbox.
            UICheckBox disableFollowRotationCheck = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_OPT_DFR"));
            disableFollowRotationCheck.isChecked = ModSettings.DisableFollowRotation;
            disableFollowRotationCheck.eventCheckChanged += (control, value) => { ModSettings.DisableFollowRotation = value; };
        }


        /// <summary>
        /// Adds a distance slider.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="yPos">Relative y-position indicator (will be incremented with slider height)</param>
        /// <param name="labelKey">Translation key for slider label</param>
        /// <param name="minValue">Slider minimum value</param>
        /// <param name="maxValue">Slider maximum value</param>
        /// <param name="initialValue">Initial slider value</param>
        /// <returns>New delay slider with attached game-time label</returns>
        private UISlider AddDistanceSlider(UIComponent parent, ref float yPos, string labelKey, float minValue, float maxValue, float initialValue)
        {
            // Create new slider.
            UISlider newSlider = UIControls.AddSlider(parent, Margin, yPos, Translations.Translate(labelKey), minValue, maxValue, 0.1f, initialValue);

            // Game-distanceLabel label.
            UILabel distanceLabel = UIControls.AddLabel(newSlider.parent, Margin, newSlider.parent.height - 15f, string.Empty);
            newSlider.objectUserData = distanceLabel;

            // Force set slider value to populate initial time label and add event handler.
            SetDistanceLabel(newSlider, initialValue);
            newSlider.eventValueChanged += SetDistanceLabel;

            // Increment y position indicator.
            yPos += newSlider.parent.height + distanceLabel.height + Margin;

            return newSlider;
        }


        /// <summary>
        /// Adds a generic value slider.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="yPos">Relative y-position indicator (will be incremented with slider height)</param>
        /// <param name="labelKey">Translation key for slider label</param>
        /// <param name="minValue">Slider minimum value</param>
        /// <param name="maxValue">Slider maximum value</param>
        /// <param name="initialValue">Initial slider value</param>
        /// <returns>New delay slider with attached game-time label</returns>
        private UISlider AddSlider(UIComponent parent, ref float yPos, string labelKey, float minValue, float maxValue, float initialValue)
        {
            // Create new slider.
            UISlider newSlider = UIControls.AddSlider(parent, Margin, yPos, Translations.Translate(labelKey), minValue, maxValue, 1f, initialValue);

            // Value label.
            UILabel valueLabel = UIControls.AddLabel(newSlider.parent, Margin, newSlider.parent.height - 15f, string.Empty);
            newSlider.objectUserData = valueLabel;

            // Force set slider value to populate initial time label and add event handler.
            SetSliderLabel(newSlider, initialValue);
            newSlider.eventValueChanged += SetSliderLabel;

            // Increment y position indicator.
            yPos += newSlider.parent.height + valueLabel.height + Margin;

            return newSlider;
        }


        /// <summary>
        /// Sets the distance value label text for a distance slider.
        /// </summary>
        /// <param name="control">Calling component</param>
        /// <param name="value">New value</param>
        private void SetDistanceLabel(UIComponent control, float value)
        {
            // Ensure that there's a valid label attached to the slider.
            if (control.objectUserData is UILabel label)
            {
                label.text = value.RoundToNearest(0.1f).ToString("N1") + "m";
            }
        }


        /// <summary>
        /// Sets the value label text for a generic slider.
        /// </summary>
        /// <param name="control">Calling component</param>
        /// <param name="value">New value</param>
        private void SetSliderLabel(UIComponent control, float value)
        {
            // Ensure that there's a valid label attached to the slider.
            if (control.objectUserData is UILabel label)
            {
                label.text = value.RoundToNearest(1f).ToString("N0");
            }
        }
    }
}