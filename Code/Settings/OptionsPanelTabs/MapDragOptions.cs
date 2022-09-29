// <copyright file="MapDragOptions.cs" company="algernon (K. Algernon A. Sheppard)">
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
    /// Options panel for setting map dragging options.
    /// </summary>
    internal class MapDragOptions
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapDragOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal MapDragOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("CAM_OPT_MDG"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = GroupMargin;

            // Mouse drag movement key control.
            OptionsKeymapping mdKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            mdKeyMapping.Label = Translations.Translate("KEY_MDG");
            mdKeyMapping.Binding = ModSettings.MapDragKey;
            mdKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += mdKeyMapping.Panel.height + GroupMargin;

            // Invert x axis checkbox.
            UICheckBox invertXCheck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_MDG_INX"));
            invertXCheck.isChecked = MapDragging.InvertXDrag;
            invertXCheck.eventCheckChanged += (c, value) => { MapDragging.InvertXDrag = value; };
            currentY += invertXCheck.height + Margin;

            // Invert y axis checkbox.
            UICheckBox invertYCheck = UICheckBoxes.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_MDG_INY"));
            invertYCheck.isChecked = MapDragging.InvertYDrag;
            invertYCheck.eventCheckChanged += (c, value) => { MapDragging.InvertYDrag = value; };
            currentY += invertYCheck.height + GroupMargin;

            // Drag speed slider .
            UISlider dragSpeedSlider = UISliders.AddPlainSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_MDG_SPD"), MapDragging.MinDragSpeed, MapDragging.MaxDragSpeed, 0.1f, MapDragging.DragSpeed);
            dragSpeedSlider.eventValueChanged += (c, value) => { MapDragging.DragSpeed = value; };
        }
    }
}