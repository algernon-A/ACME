using UnityEngine;
using ColossalFramework.UI;


namespace ACME
{
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
        /// Adds mod options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
        internal MapDragOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = PanelUtils.AddTab(tabStrip, Translations.Translate("CAM_OPT_MDG"), tabIndex, false);

            // Add controls.

            // Y position indicator.
            float currentY = GroupMargin;

            // Mouse drag movement key control.
            OptionsKeymapping mdKeyMapping = panel.gameObject.AddComponent<MapDragKeymapping>();
            mdKeyMapping.uIPanel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += mdKeyMapping.uIPanel.height + GroupMargin;

            // Invert x axis checkbox.
            UICheckBox invertXCheck = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_MDG_INX"));
            invertXCheck.isChecked = MapDragging.InvertXDrag;
            invertXCheck.eventCheckChanged += (control, value) => { MapDragging.InvertXDrag = value; };
            currentY += invertXCheck.height + Margin;

            // Invert y axis checkbox.
            UICheckBox invertYCheck = UIControls.AddPlainCheckBox(panel, Margin, currentY, Translations.Translate("CAM_MDG_INY"));
            invertYCheck.isChecked = MapDragging.InvertYDrag;
            invertYCheck.eventCheckChanged += (control, value) => { MapDragging.InvertYDrag = value; };
            currentY += invertYCheck.height + GroupMargin;

            // Drag speed slider .
            UISlider dragSpeedSlider = UIControls.AddSliderWithValue(panel, Margin, currentY, Translations.Translate("CAM_MDG_SPD"), MapDragging.MinDragSpeed, MapDragging.MaxDragSpeed, 0.1f, MapDragging.DragSpeed, (value) => { MapDragging.DragSpeed = value; });
        }
    }
}