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
            AutoTabstrip tabStrip = AutoTabstrip.AddTabstrip(this, 0f, 0f, OptionsPanelManager<OptionsPanel>.PanelWidth, OptionsPanelManager<OptionsPanel>.PanelHeight, out _, tabHeight: 50f);

            // Add tabs and panels.
            new GeneralOptions(tabStrip, 0);
            new HotkeyOptions(tabStrip, 1);
            new CameraOptions(tabStrip, 2);
            new ShadowOptions(tabStrip, 3);
            new MapDragOptions(tabStrip, 4);
            new FPSOptions(tabStrip, 5);

            // Select first tab.
            tabStrip.selectedIndex = -1;
            tabStrip.selectedIndex = 0;
        }
    }
}