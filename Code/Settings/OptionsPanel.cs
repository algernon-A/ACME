// <copyright file="OptionsPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons.UI;
    using ColossalFramework.UI;

    /// <summary>
    /// Class to handle the mod's options panel.
    /// </summary>
    public class OptionsPanel : UIPanel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsPanel"/> class.
        /// </summary>
        internal OptionsPanel()
        {
            // Add tabstrip.
            UITabstrip tabStrip = UITabstrips.AddTabStrip(this, 0f, 0f, OptionsPanelManager<OptionsPanel>.PanelWidth, OptionsPanelManager<OptionsPanel>.PanelHeight, out _);

            // Add tabs and panels.
            new GeneralOptions(tabStrip, 0);
            new FPSOptions(tabStrip, 1);
            new MapDragOptions(tabStrip, 2);
            new ShadowOptions(tabStrip, 3);

            // Select first tab.
            tabStrip.selectedIndex = -1;
            tabStrip.selectedIndex = 0;
        }
    }
}