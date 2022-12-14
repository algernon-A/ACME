// <copyright file="GeneralOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons;
    using AlgernonCommons.Keybinding;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

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
        /// Initializes a new instance of the <see cref="GeneralOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal GeneralOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("CAM_OPT_GEN"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = Margin;

            // Language choice.
            UIDropDown languageDropDown = UIDropDowns.AddPlainDropDown(panel, LeftMargin, currentY, Translations.Translate("LANGUAGE_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (c, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
            };
            currentY += languageDropDown.parent.height + Margin;

            // Hotkey control.
            OptionsKeymapping uuiKeyMapping = languageDropDown.parent.parent.gameObject.AddComponent<UUIKeymapping>();
            uuiKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += uuiKeyMapping.Panel.height + Margin;

            // Building collision checkbox.
            UICheckBox buildingCollisionCheck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_BLD"));
            buildingCollisionCheck.isChecked = HeightOffset.BuildingCollision;
            buildingCollisionCheck.eventCheckChanged += (c, value) => { HeightOffset.BuildingCollision = value; };
            currentY += buildingCollisionCheck.height + Margin;

            // Network collision checkbox.
            UICheckBox netCollisionCHeck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_NET"));
            netCollisionCHeck.isChecked = HeightOffset.NetworkCollision;
            netCollisionCHeck.eventCheckChanged += (c, value) => { HeightOffset.NetworkCollision = value; };
            currentY += netCollisionCHeck.height + Margin;

            // Prop collision checkbox.
            UICheckBox propCollisionCHeck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_PRO"));
            propCollisionCHeck.isChecked = HeightOffset.PropCollision;
            propCollisionCHeck.eventCheckChanged += (c, value) => { HeightOffset.PropCollision = value; };
            currentY += propCollisionCHeck.height + Margin;

            // Tree collision checkbox.
            UICheckBox treeCollisionCheck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_TRE"));
            treeCollisionCheck.isChecked = HeightOffset.TreeCollision;
            treeCollisionCheck.eventCheckChanged += (c, value) => { HeightOffset.TreeCollision = value; };
            currentY += propCollisionCHeck.height + Margin;

            // Water bobbing checkbox.
            UICheckBox waterBobbingCheck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_COL_WAT"));
            waterBobbingCheck.isChecked = UpdateTargetPosition.WaterBobbing;
            waterBobbingCheck.eventCheckChanged += (c, value) => { UpdateTargetPosition.WaterBobbing = value; };
            currentY += propCollisionCHeck.height + GroupMargin;

            // Ground proximity slider .
            UISlider groundProximitySlider = AddDistanceSlider(panel, ref currentY, "CAM_COL_GND", HeightOffset.MinTerrainClearance, HeightOffset.MaxTerrainClearance, HeightOffset.TerrainClearance);
            groundProximitySlider.eventValueChanged += (c, value) => { HeightOffset.TerrainClearance = value; };

            // Near clipping slider.
            UISlider nearClipSlider = AddDistanceSlider(panel, ref currentY, "CAM_CLP_NEA", CameraUtils.MinNearClip, CameraUtils.MaxNearClip, CameraUtils.NearClipPlane);
            nearClipSlider.eventValueChanged += (c, value) => { CameraUtils.NearClipPlane = value; };

            // Speed multiplier.
            UISlider speedSlider = AddSlider(panel, ref currentY, "CAM_SPD_MIN", UpdateTargetPosition.MinCameraSpeed, UpdateTargetPosition.MaxCameraSpeed, UpdateTargetPosition.CameraSpeed);
            speedSlider.eventValueChanged += (c, value) => { UpdateTargetPosition.CameraSpeed = value; };
            currentY += Margin;

            // Follow disasters checkbox.
            UICheckBox disableDisasterGoto = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_OPT_DIS"));
            disableDisasterGoto.isChecked = FollowDisasterPatch.FollowDisasters;
            disableDisasterGoto.eventCheckChanged += (c, value) => { FollowDisasterPatch.FollowDisasters = value; };
            currentY += disableDisasterGoto.height + Margin;

            // Zoom to cursor.
            UICheckBox zoomToCursorCheck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_OPT_ZTC"));
            zoomToCursorCheck.isChecked = ModSettings.ZoomToCursor;
            zoomToCursorCheck.eventCheckChanged += (c, value) => { ModSettings.ZoomToCursor = value; };
            zoomToCursorCheck.tooltipBox = UIToolTips.WordWrapToolTip;
            zoomToCursorCheck.tooltip = Translations.Translate("CAM_OPT_ZTC_TIP");
            currentY += zoomToCursorCheck.height + Margin;

            // Follow disasters checkbox.
            UICheckBox disableFollowRotationCheck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_OPT_DFR"));
            disableFollowRotationCheck.isChecked = ModSettings.DisableFollowRotation;
            disableFollowRotationCheck.eventCheckChanged += (c, value) => { ModSettings.DisableFollowRotation = value; };
            currentY += disableFollowRotationCheck.height + 30f;

            // Logging checkbox.
            UICheckBox loggingCheck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("DETAIL_LOGGING"));
            loggingCheck.isChecked = Logging.DetailLogging;
            loggingCheck.eventCheckChanged += (c, isChecked) => { Logging.DetailLogging = isChecked; };
        }

        /// <summary>
        /// Adds a distance slider.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="yPos">Relative y-position indicator (will be incremented with slider height).</param>
        /// <param name="labelKey">Translation key for slider label.</param>
        /// <param name="minValue">Slider minimum value.</param>
        /// <param name="maxValue">Slider maximum value.</param>
        /// <param name="initialValue">Initial slider value.</param>
        /// <returns>New delay slider with attached game-time label.</returns>
        private UISlider AddDistanceSlider(UIComponent parent, ref float yPos, string labelKey, float minValue, float maxValue, float initialValue)
        {
            // Create new slider.
            UISlider newSlider = UISliders.AddPlainSlider(parent, Margin, yPos, Translations.Translate(labelKey), minValue, maxValue, 0.1f, initialValue);

            // Game-distanceLabel label.
            UILabel distanceLabel = UILabels.AddLabel(newSlider.parent, Margin, newSlider.parent.height - 15f, string.Empty);
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
        /// <param name="parent">Parent component.</param>
        /// <param name="yPos">Relative y-position indicator (will be incremented with slider height).</param>
        /// <param name="labelKey">Translation key for slider label.</param>
        /// <param name="minValue">Slider minimum value.</param>
        /// <param name="maxValue">Slider maximum value.</param>
        /// <param name="initialValue">Initial slider value.</param>
        /// <returns>New delay slider with attached game-time label.</returns>
        private UISlider AddSlider(UIComponent parent, ref float yPos, string labelKey, float minValue, float maxValue, float initialValue)
        {
            // Create new slider.
            UISlider newSlider = UISliders.AddPlainSlider(parent, Margin, yPos, Translations.Translate(labelKey), minValue, maxValue, 1f, initialValue);

            // Value label.
            UILabel valueLabel = UILabels.AddLabel(newSlider.parent, Margin, newSlider.parent.height - 15f, string.Empty);
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
        /// <param name="control">Calling component.</param>
        /// <param name="value">New value.</param>
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
        /// <param name="control">Calling component.</param>
        /// <param name="value">New value.</param>
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