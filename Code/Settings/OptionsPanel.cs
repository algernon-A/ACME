// <copyright file="OptionsPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons.UI;

    /// <summary>
    /// Class to handle the mod's options panel.
    /// </summary>
    public class OptionsPanel : OptionsPanelBase
    {
        /// <summary>
        /// Performs on-demand panel setup.
        /// </summary>
        protected override void Setup()
        {
            // Add tabstrip.
            AutoTabstrip tabStrip = AutoTabstrip.AddTabstrip(this, 0f, 0f, OptionsPanelManager<OptionsPanel>.PanelWidth, OptionsPanelManager<OptionsPanel>.PanelHeight, out _, tabHeight: 50f);

            // Add tabs and panels.
            new GeneralOptions(tabStrip, 0);
            new HotkeyOptions(tabStrip, 1);
            new FPSOptions(tabStrip, 2);
            new OtherOptions(tabStrip, 3);

            // Select first tab.
            tabStrip.selectedIndex = -1;
            tabStrip.selectedIndex = 0;
        }
    }
}