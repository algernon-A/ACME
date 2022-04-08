using ColossalFramework.UI;


namespace ACME
{
    /// <summary>
    /// Utilities for Options Panel UI.
    /// </summary>
    internal static class PanelUtils
    {
        /// <summary>
        /// Adds a tab to a UI tabstrip.
        /// </summary>
        /// <param name="tabStrip">UIT tabstrip to add to</param>
        /// <param name="tabName">Name of this tab</param>
        /// <param name="tabIndex">Index number of this tab</param>
        /// <param name="autoLayout">Autolayout</param>
        /// <returns>UIHelper instance for the new tab panel</returns>
        internal static UIPanel AddTab(UITabstrip tabStrip, string tabName, int tabIndex, bool autoLayout)
        {
            // Create tab.
            UIButton tabButton = tabStrip.AddTab(tabName);

            // Sprites.
            tabButton.normalBgSprite = "SubBarButtonBase";
            tabButton.disabledBgSprite = "SubBarButtonBaseDisabled";
            tabButton.focusedBgSprite = "SubBarButtonBaseFocused";
            tabButton.hoveredBgSprite = "SubBarButtonBaseHovered";
            tabButton.pressedBgSprite = "SubBarButtonBasePressed";

            // Tooltip.
            tabButton.tooltip = tabName;

            tabStrip.selectedIndex = tabIndex;

            // Force width.
            tabButton.width = 200;

            // Get tab root panel.
            UIPanel rootPanel = tabStrip.tabContainer.components[tabIndex] as UIPanel;

            // Autolayout.
            rootPanel.autoLayout = autoLayout;

            if (autoLayout)
            {
                rootPanel.autoLayoutDirection = LayoutDirection.Vertical;
                rootPanel.autoLayoutPadding.top = 5;
                rootPanel.autoLayoutPadding.left = 10;
            }

            return rootPanel;
        }
    }
}