// <copyright file="PositionKeyOptions.cs" company="algernon (K. Algernon A. Sheppard)">
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
    /// Options panel for setting customized hotkeys.
    /// </summary>
    internal class PositionKeyOptions
    {
        // Layout constants.x
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionKeyOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal PositionKeyOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("CAM_OPT_POS"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = Margin;

            // Add scrollable panel for hotkeys.
            UIScrollablePanel scrollPanel = panel.AddUIComponent<UIScrollablePanel>();
            scrollPanel.relativePosition = new Vector2(0, currentY);
            scrollPanel.autoSize = false;
            scrollPanel.autoLayout = false;
            scrollPanel.width = panel.width - GroupMargin;
            scrollPanel.height = panel.height - currentY - GroupMargin;
            scrollPanel.clipChildren = true;
            scrollPanel.builtinKeyNavigation = true;
            scrollPanel.scrollWheelDirection = UIOrientation.Vertical;
            UIScrollbars.AddScrollbar(panel, scrollPanel);

            for (int i = 0; i < UIThreading.NumSlots; ++i)
            {
                OptionsKeymapping loadKeyMapping = scrollPanel.gameObject.AddComponent<OptionsKeymapping>();
                loadKeyMapping.Label = $"{Translations.Translate("CAM_LOA_POS")} {i + 1}";
                loadKeyMapping.Panel.tooltip = $"{Translations.Translate("CAM_LOA_POS_TIP")} {i + 1}";
                loadKeyMapping.Binding = UIThreading.LoadKeyBindings[i];
                loadKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
                currentY += loadKeyMapping.Panel.height + Margin;
            }

            currentY += GroupMargin;

            for (int i = 0; i < UIThreading.NumSlots; ++i)
            {
                OptionsKeymapping saveKeyMapping = scrollPanel.gameObject.AddComponent<OptionsKeymapping>();
                saveKeyMapping.Label = $"{Translations.Translate("CAM_SAV_POS")} {i + 1}";
                saveKeyMapping.Panel.tooltip = $"{Translations.Translate("CAM_SAV_POS_TIP")} {i + 1}";
                saveKeyMapping.Binding = UIThreading.SaveKeyBindings[i];
                saveKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
                currentY += saveKeyMapping.Panel.height + Margin;
            }
        }
    }
}