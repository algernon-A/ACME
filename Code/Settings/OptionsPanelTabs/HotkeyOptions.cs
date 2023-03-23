// <copyright file="HotkeyOptions.cs" company="algernon (K. Algernon A. Sheppard)">
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
    internal class HotkeyOptions
    {
        // Layout constants.x
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal HotkeyOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("TAB_KEYS"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = GroupMargin;

            // Panel hotkey control.
            OptionsKeymapping uuiKeyMapping = panel.gameObject.AddComponent<UUIKeymapping>();
            uuiKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += uuiKeyMapping.Panel.height + GroupMargin;

            // FPS mode key control.
            OptionsKeymapping fpsKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            fpsKeyMapping.Label = Translations.Translate("KEY_FPS");
            fpsKeyMapping.Binding = UIThreading.FPSModeKey;
            fpsKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += fpsKeyMapping.Panel.height + GroupMargin;

            // Reset position key control.
            OptionsKeymapping resetKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            resetKeyMapping.Label = Translations.Translate("KEY_RESET");
            resetKeyMapping.Panel.tooltip = Translations.Translate("KEY_RESET_TIP");
            resetKeyMapping.Binding = UIThreading.ResetKey;
            resetKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += resetKeyMapping.Panel.height + GroupMargin;

            // MoveIt key control.
            OptionsKeymapping miKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            miKeyMapping.Label = Translations.Translate("KEY_GOTO_MOVEIT");
            miKeyMapping.Panel.tooltip = Translations.Translate("KEY_GOTO_MOVEIT_TIP");
            miKeyMapping.Binding = UIThreading.MoveItKey;
            miKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += miKeyMapping.Panel.height + GroupMargin;

            // Fixed X-rotation key.
            OptionsKeymapping xKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            xKeyMapping.Label = Translations.Translate("KEY_XROT");
            xKeyMapping.Panel.tooltip = Translations.Translate("KEY_ROT_TIP");
            xKeyMapping.Panel.tooltipBox = UIToolTips.WordWrapToolTip;
            xKeyMapping.Binding = UIThreading.XKey;
            xKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += xKeyMapping.Panel.height + GroupMargin;

            // Fixed Y-rotation key.
            OptionsKeymapping yKeyMapping = panel.gameObject.AddComponent<OptionsKeymapping>();
            yKeyMapping.Label = Translations.Translate("KEY_YROT");
            yKeyMapping.Panel.tooltip = Translations.Translate("KEY_ROT_TIP");
            yKeyMapping.Panel.tooltipBox = UIToolTips.WordWrapToolTip;
            yKeyMapping.Binding = UIThreading.YKey;
            yKeyMapping.Panel.relativePosition = new Vector2(LeftMargin, currentY);
        }
    }
}