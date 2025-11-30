// <copyright file="OtherOptions.cs" company="algernon (K. Algernon A. Sheppard)">
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
    /// Options panel for setting shadow options.
    /// </summary>
    internal class OtherOptions
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;
        private const float TitleMargin = 55f;

        /// <summary>
        /// Initializes a new instance of the <see cref="OtherOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal OtherOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("CAM_OPT_OTH"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = GroupMargin;

            // Shadow options.
            float headerWidth = OptionsPanelManager<OptionsPanel>.PanelWidth - (Margin * 2f);
            UISpacers.AddTitleSpacer(panel, Margin, currentY, headerWidth, Translations.Translate("CAM_OPT_SHD"));
            currentY += TitleMargin;

            // Shadow distance slider .
            UISlider maxShadowDistanceSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("CAM_SHD_MAX"), CameraUtils.MinMaxShadowDistance, CameraUtils.MaxMaxShadowDistance, 1000f, CameraUtils.MaxShadowDistance);
            maxShadowDistanceSlider.eventValueChanged += (c, value) => { CameraUtils.MaxShadowDistance = value; };
            currentY += maxShadowDistanceSlider.parent.height;

            // Shadow sharpness tradeoff slider .
            UISlider minShadowDistanceSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("CAM_SHD_MIN"), CameraUtils.MinMinShadowDistance, CameraUtils.MaxMinShadowDistance, 10f, CameraUtils.MinShadowDistance);
            minShadowDistanceSlider.eventValueChanged += (c, value) => { CameraUtils.MinShadowDistance = value; };
            UILabels.AddLabel(minShadowDistanceSlider, 0f, minShadowDistanceSlider.height + Margin, Translations.Translate("SHARP"), textScale: 0.7f);
            UILabels.AddLabel(minShadowDistanceSlider, minShadowDistanceSlider.width, minShadowDistanceSlider.height + Margin, Translations.Translate("FUZZY"), textScale: 0.7f, alignment: UIHorizontalAlignment.Right);
            currentY += minShadowDistanceSlider.parent.height + GroupMargin;

            // Map drag options.
            UISpacers.AddTitleSpacer(panel, Margin, currentY, headerWidth, Translations.Translate("CAM_OPT_MDG"));
            currentY += TitleMargin;

            // Mouse drag movement key control.
            OptionsKeymapping mdKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            mdKeyMapping.Label = Translations.Translate("KEY_MDG");
            mdKeyMapping.Binding = ModSettings.MapDragKey;
            mdKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += mdKeyMapping.Panel.height + Margin;

            // Drag speed slider .
            UISlider dragSpeedSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("CAM_MDG_SPD"), MapDragging.MinDragSpeed, MapDragging.MaxDragSpeed, 0.1f, MapDragging.DragSpeed);
            dragSpeedSlider.eventValueChanged += (c, value) => { MapDragging.DragSpeed = value; };
            currentY += dragSpeedSlider.parent.height;

            // Invert x axis checkbox.
            UICheckBox invertXCheck = UICheckBoxes.AddPlainCheckBox(panel, LeftMargin, currentY, Translations.Translate("CAM_MDG_INX"));
            invertXCheck.isChecked = MapDragging.InvertXDrag;
            invertXCheck.eventCheckChanged += (c, value) => { MapDragging.InvertXDrag = value; };
            currentY += invertXCheck.height + Margin;

            // Invert y axis checkbox.
            UICheckBox invertYCheck = UICheckBoxes.AddPlainCheckBox(panel, LeftMargin, currentY, Translations.Translate("CAM_MDG_INY"));
            invertYCheck.isChecked = MapDragging.InvertYDrag;
            invertYCheck.eventCheckChanged += (c, value) => { MapDragging.InvertYDrag = value; };
            currentY += invertYCheck.height + Margin;

            // Miscellaneous options options.
            UISpacers.AddTitleSpacer(panel, Margin, currentY, headerWidth, Translations.Translate("CAM_OPT_MIS"));
            currentY += TitleMargin;

            // Follow disasters checkbox.
            UICheckBox disableDisasterGoto = UICheckBoxes.AddPlainCheckBox(panel, LeftMargin, currentY, Translations.Translate("CAM_OPT_DIS"));
            disableDisasterGoto.isChecked = FollowDisasterPatch.FollowDisasters;
            disableDisasterGoto.eventCheckChanged += (c, value) => { FollowDisasterPatch.FollowDisasters = value; };
            currentY += disableDisasterGoto.height + Margin;

            // Logging checkbox.
            UICheckBox loggingCheck = UICheckBoxes.AddPlainCheckBox(panel, LeftMargin, currentY, Translations.Translate("DETAIL_LOGGING"));
            loggingCheck.isChecked = Logging.DetailLogging;
            loggingCheck.eventCheckChanged += (c, isChecked) => { Logging.DetailLogging = isChecked; };
            currentY += TitleMargin;

            // Erase positions button - only valid when a game is loaded.
            if (Loading.IsLoaded)
            {
                UIButton erasePositionsButton = UIButtons.AddButton(panel, LeftMargin, currentY, Translations.Translate("CAM_ERS_POS"), width: 300f, tooltip: Translations.Translate("CAM_ERS_POS_TIP"));
                erasePositionsButton.eventClick += (c, p) => { CameraPositions.ErasePositions(); };
            }
        }
    }
}