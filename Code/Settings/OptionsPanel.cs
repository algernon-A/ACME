using UnityEngine;
using ColossalFramework.UI;


namespace ACME
{
    /// <summary>
    /// ACME options panel.
    /// </summary>
    public class ACMEOptionsPanel : UIPanel
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;


        /// <summary>
        /// Performs initial setup for the panel; we don't use Start() as that's not sufficiently reliable (race conditions), and is not needed with the dynamic create/destroy process.
        /// </summary>
        internal void Setup()
        {
            // Add controls.
            this.autoLayout = false;

            // Y position indicator.
            float currentY = Margin;

            // Language choice.
            UIDropDown languageDropDown = UIControls.AddPlainDropDown(this, Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager.LocaleChanged();
            };
            languageDropDown.parent.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += languageDropDown.parent.height + GroupMargin;

            // Hotkey control.
            OptionsKeymapping keyMapping = languageDropDown.parent.parent.gameObject.AddComponent<OptionsKeymapping>();
            keyMapping.uIPanel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += keyMapping.uIPanel.height + Margin;

            // MoveIt key control.
            OptionsKeymapping miKeyMapping = languageDropDown.parent.parent.gameObject.AddComponent<MoveItKeymapping>();
            miKeyMapping.uIPanel.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += miKeyMapping.uIPanel.height + GroupMargin;

            // Building collision checkbox.
            UICheckBox buildingCollisionCheck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("CAM_COL_BLD"));
            buildingCollisionCheck.isChecked = HeightOffset.buildingCollision;
            buildingCollisionCheck.eventCheckChanged += (control, value) => { HeightOffset.buildingCollision = value; };
            currentY += buildingCollisionCheck.height + Margin;

            // Network collision checkbox.
            UICheckBox netCollisionCHeck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("CAM_COL_NET"));
            netCollisionCHeck.isChecked = HeightOffset.networkCollision;
            netCollisionCHeck.eventCheckChanged += (control, value) => { HeightOffset.networkCollision = value; };
            currentY += netCollisionCHeck.height + Margin;

            // Prop collision checkbox.
            UICheckBox propCollisionCHeck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("CAM_COL_PRO"));
            propCollisionCHeck.isChecked = HeightOffset.propCollision;
            propCollisionCHeck.eventCheckChanged += (control, value) => { HeightOffset.propCollision = value; };
            currentY += propCollisionCHeck.height + Margin;

            // Tree collision checkbox.
            UICheckBox treeCollisionCheck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("CAM_COL_TRE"));
            treeCollisionCheck.isChecked = HeightOffset.treeCollision;
            treeCollisionCheck.eventCheckChanged += (control, value) => { HeightOffset.treeCollision = value; };
            currentY += propCollisionCHeck.height + Margin;

            // Water bobbing checkbox.
            UICheckBox waterBobbingCheck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("CAM_COL_WAT"));
            waterBobbingCheck.isChecked = HeightOffset.waterBobbing;
            waterBobbingCheck.eventCheckChanged += (control, value) => { HeightOffset.waterBobbing = value; };
            currentY += propCollisionCHeck.height + GroupMargin;

            // Ground proximity slider .
            UISlider groundProximitySlider = AddDistanceSlider(ref currentY, "CAM_COL_GND", HeightOffset.MinTerrainClearance, HeightOffset.MaxTerrainClearance, HeightOffset.TerrainClearance);
            groundProximitySlider.eventValueChanged += (control, value) => { HeightOffset.TerrainClearance = value; };

            // Near clipping slider.
            UISlider nearClipSlider = AddDistanceSlider(ref currentY, "CAM_CLP_NEA", CameraUtils.MinNearClip, CameraUtils.MaxNearClip, CameraUtils.NearClipPlane);
            nearClipSlider.eventValueChanged += (control, value) => { CameraUtils.NearClipPlane = value; };

            // Speed multiplier.
            UISlider speedSlider = AddSlider(ref currentY, "CAM_SPD_MIN", UpdateTargetPosition.MinCameraSpeed, UpdateTargetPosition.MaxCameraSpeed, UpdateTargetPosition.CameraSpeed);
            speedSlider.eventValueChanged += (control, value) => { UpdateTargetPosition.CameraSpeed = value; };

            // Follow disasters checkbox.
            currentY += Margin;
            UICheckBox disableDisasterGoto = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("CAM_OPT_DIS"));
            disableDisasterGoto.isChecked = FollowDisasterPatch.followDisasters;
            disableDisasterGoto.eventCheckChanged += (control, value) => { FollowDisasterPatch.followDisasters = value; };

        }


        /// <summary>
        /// Adds a distance slider.
        /// </summary>
        /// <param name="yPos">Relative y-position indicator (will be incremented with slider height)</param>
        /// <param name="labelKey">Translation key for slider label</param>
        /// <param name="minValue">Slider minimum value</param>
        /// <param name="maxValue">Slider maximum value</param>
        /// <param name="initialValue">Initial slider value</param>
        /// <returns>New delay slider with attached game-time label</returns>
        private UISlider AddDistanceSlider(ref float yPos, string labelKey, float minValue, float maxValue, float initialValue)
        {
            // Create new slider.
            UISlider newSlider = UIControls.AddSlider(this, Translations.Translate(labelKey), minValue, maxValue, 0.1f, initialValue);
            newSlider.parent.relativePosition = new Vector2(Margin, yPos);

            // Game-distanceLabel label.
            UILabel distanceLabel = UIControls.AddLabel(newSlider.parent, Margin, newSlider.parent.height - 15f, string.Empty);
            newSlider.objectUserData = distanceLabel;

            // Force set slider value to populate initial time label and add event handler.
            SetDistanceLabel(newSlider, initialValue);
            newSlider.eventValueChanged += SetDistanceLabel;

            // Increment y position indicator.
            yPos += newSlider.parent.height + distanceLabel.height + Margin;

            return newSlider;
        }


        /// <summary>
        /// Adds a generic value slider.
        /// </summary>
        /// <param name="yPos">Relative y-position indicator (will be incremented with slider height)</param>
        /// <param name="labelKey">Translation key for slider label</param>
        /// <param name="minValue">Slider minimum value</param>
        /// <param name="maxValue">Slider maximum value</param>
        /// <param name="initialValue">Initial slider value</param>
        /// <returns>New delay slider with attached game-time label</returns>
        private UISlider AddSlider(ref float yPos, string labelKey, float minValue, float maxValue, float initialValue)
        {
            // Create new slider.
            UISlider newSlider = UIControls.AddSlider(this, Translations.Translate(labelKey), minValue, maxValue, 1f, initialValue);
            newSlider.parent.relativePosition = new Vector2(Margin, yPos);

            // Value label.
            UILabel valueLabel = UIControls.AddLabel(newSlider.parent, Margin, newSlider.parent.height - 15f, string.Empty);
            newSlider.objectUserData = valueLabel;

            // Force set slider value to populate initial time label and add event handler.
            SetSliderLabel(newSlider, initialValue);
            newSlider.eventValueChanged += SetSliderLabel;

            // Increment y position indicator.
            yPos += newSlider.parent.height + valueLabel.height + Margin;

            return newSlider;
        }


        /// <summary>
        /// Sets the distance value label text for a distance slider.
        /// </summary>
        /// <param name="control">Calling component</param>
        /// <param name="value">New value</param>
        private void SetDistanceLabel(UIComponent control, float value)
        {
            // Ensure that there's a valid label attached to the slider.
            if (control.objectUserData is UILabel label)
            {
                label.text = value.RoundToNearest(0.1f).ToString("N1") + "m";
            }
        }


        /// <summary>
        /// Sets the value label text for a generic slider.
        /// </summary>
        /// <param name="control">Calling component</param>
        /// <param name="value">New value</param>
        private void SetSliderLabel(UIComponent control, float value)
        {
            // Ensure that there's a valid label attached to the slider.
            if (control.objectUserData is UILabel label)
            {
                label.text = value.RoundToNearest(1f).ToString("N0");
            }
        }
    }
}