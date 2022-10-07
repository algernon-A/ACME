// <copyright file="ShadowOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Options panel for setting shadow options.
    /// </summary>
    internal class ShadowOptions
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float GroupMargin = 40f;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShadowOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal ShadowOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("CAM_OPT_SHD"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = GroupMargin;

            // Shadow distance slider .
            UISlider maxShadowDistanceSlider = UISliders.AddPlainSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_SHD_MAX"), CameraUtils.MinMaxShadowDistance, CameraUtils.MaxMaxShadowDistance, 1000f, CameraUtils.MaxShadowDistance);
            maxShadowDistanceSlider.eventValueChanged += (c, value) => { CameraUtils.MaxShadowDistance = value; };
            currentY += maxShadowDistanceSlider.parent.height;

            // Shadow distance slider .
            UISlider minShadowDistanceSlider = UISliders.AddPlainSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_SHD_MIN"), CameraUtils.MinMinShadowDistance, CameraUtils.MaxMinShadowDistance, 10f, CameraUtils.MinShadowDistance);
            minShadowDistanceSlider.eventValueChanged += (c, value) => { CameraUtils.MinShadowDistance = value; };
            UILabels.AddLabel(minShadowDistanceSlider, 0f, minShadowDistanceSlider.height + Margin, Translations.Translate("SHARP"), textScale: 0.7f);
            UILabels.AddLabel(minShadowDistanceSlider, minShadowDistanceSlider.width, minShadowDistanceSlider.height + Margin, Translations.Translate("FUZZY"), textScale: 0.7f, alignment: UIHorizontalAlignment.Right);
        }
    }
}