// <copyright file="CameraPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// The ACME camera panel.
    /// </summary>
    internal class CameraPanel : StandalonePanel
    {
        // Layout constants - X.
        private const float CalculatedPanelWidth = 400f;

        // Layout constants - Y.
        private const float TitleBarHeight = 50f;
        private const float CheckHeight = 20f;
        private const float SliderHeight = 38f;
        private const float XSliderY = TitleBarHeight;
        private const float ZSliderY = XSliderY + SliderHeight;
        private const float RotSliderY = ZSliderY + SliderHeight;
        private const float ZoomSliderY = RotSliderY + SliderHeight;
        private const float TiltSliderY = ZoomSliderY + SliderHeight;
        private const float FovSliderY = TiltSliderY + SliderHeight;
        private const float ShadowMaxSliderY = FovSliderY + SliderHeight;
        private const float ShadowMinSliderY = ShadowMaxSliderY + SliderHeight;
        private const float Check1Y = ShadowMinSliderY + SliderHeight;
        private const float Check2Y = Check1Y + CheckHeight;
        private const float Check3Y = Check2Y + CheckHeight;
        private const float Check1X = Margin;
        private const float Check2X = (CalculatedPanelWidth - (Margin * 2f)) / 2f;
        private const float CalculatedPanelHeight = Check3Y + CheckHeight + Margin;

        // Camera constants.
        private const float MinFOV = 5f;
        private const float MaxFOV = 84f;

        // Camera references.
        private Camera _mainCamera;
        private CameraController _controller;

        // Panel components.
        private CameraSlider _xPosSlider;
        private CameraSlider _zPosSlider;
        private CameraSlider _rotSlider;
        private CameraSlider _zoomSlider;
        private CameraSlider _tiltSlider;
        private CameraSlider _fovSlider;
        private CameraSlider _shadowMaxSlider;
        private CameraSlider _shadowMinSlider;

        /// <summary>
        /// Gets the panel width.
        /// </summary>
        public override float PanelWidth => CalculatedPanelWidth;

        /// <summary>
        /// Gets the panel height.
        /// </summary>
        public override float PanelHeight => CalculatedPanelHeight;

        /// <summary>
        /// Gets the panel's title.
        /// </summary>
        protected override string PanelTitle => Translations.Translate("CAM_NAM");

        /// <summary>
        /// Called by Unity before the first frame is displayed.
        /// Used to perform setup.
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Press UUI button.
            UUI.UUIButton.IsPressed = true;

            SetIcon(UITextures.LoadSingleSpriteAtlas("ACME-UUI"), "normal");

            // Set camera references.
            _controller = CameraUtils.Controller;
            _mainCamera = CameraUtils.MainCamera;

            // X-position slider.
            _xPosSlider = AddCameraSlider(this, Margin, XSliderY, PanelWidth - (Margin * 2f), "CAM_XPOS", -8500f, 8500f, 0.01f, _controller.m_targetPosition.x, "N1", "xPos");

            // Z-position slider.
            _zPosSlider = AddCameraSlider(this, Margin, ZSliderY, PanelWidth - (Margin * 2f), "CAM_ZPOS", -8500f, 8500f, 0.01f, _controller.m_targetPosition.z, "N1", "zPos");

            // Rotation around target slider.
            _rotSlider = AddCameraSlider(this, Margin, RotSliderY, PanelWidth - (Margin * 2f), "CAM_ROT", -180f, 180f, 0.0001f, _controller.m_targetAngle.x, "N4", "rot");

            // Zoom slider.
            _zoomSlider = AddCameraSlider(this, Margin, ZoomSliderY, PanelWidth - (Margin * 2f), "CAM_SIZE", _controller.m_minDistance, _controller.m_maxDistance, 1f, _controller.m_targetSize, "N1", "size");

            // Tilt slider.
            _tiltSlider = AddCameraSlider(this, Margin, TiltSliderY, PanelWidth - (Margin * 2f), "CAM_TILT", -90f, 90f, 0.01f, _controller.m_targetAngle.y, "N2", "tilt");

            // FOV slider.
            _fovSlider = AddCameraSlider(this, Margin, FovSliderY, PanelWidth - (Margin * 2f), "CAM_FOV", MinFOV, MaxFOV, 0.01f, _mainCamera.fieldOfView, "N1", "fov");

            // Shadow sliders.
            _shadowMaxSlider = AddCameraSlider(this, Margin, ShadowMaxSliderY, PanelWidth - (Margin * 2f), "CAM_SHD_MAX", CameraUtils.MinMaxShadowDistance, CameraUtils.MaxMaxShadowDistance, 100f, CameraUtils.MaxShadowDistance, "N0", "shadMax");
            _shadowMinSlider = AddCameraSlider(this, Margin, ShadowMinSliderY, PanelWidth - (Margin * 2f), "CAM_SHD_MIN", CameraUtils.MinMinShadowDistance, CameraUtils.MaxMinShadowDistance, 10f, CameraUtils.MinShadowDistance, "N0", "shadMin");

            // Building collision checkbox.
            UICheckBox buildingCollisionCheck = UICheckBoxes.AddLabelledCheckBox(this, Check1X, Check1Y, Translations.Translate("CAM_COL_BLD"));
            buildingCollisionCheck.isChecked = HeightOffset.BuildingCollision;
            buildingCollisionCheck.eventCheckChanged += (c, value) => { HeightOffset.BuildingCollision = value; };

            // Network collision checkbox.
            UICheckBox netCollisionCHeck = UICheckBoxes.AddLabelledCheckBox(this, Check1X, Check2Y, Translations.Translate("CAM_COL_NET"));
            netCollisionCHeck.isChecked = HeightOffset.NetworkCollision;
            netCollisionCHeck.eventCheckChanged += (c, value) => { HeightOffset.NetworkCollision = value; };

            // Prop collision checkbox.
            UICheckBox propCollisionCHeck = UICheckBoxes.AddLabelledCheckBox(this, Check2X, Check1Y, Translations.Translate("CAM_COL_PRO"));
            propCollisionCHeck.isChecked = HeightOffset.PropCollision;
            propCollisionCHeck.eventCheckChanged += (c, value) => { HeightOffset.PropCollision = value; };

            // Tree collision checkbox.
            UICheckBox treeCollisionCheck = UICheckBoxes.AddLabelledCheckBox(this, Check2X, Check2Y, Translations.Translate("CAM_COL_TRE"));
            treeCollisionCheck.isChecked = HeightOffset.TreeCollision;
            treeCollisionCheck.eventCheckChanged += (c, value) => { HeightOffset.TreeCollision = value; };

            // Zoom to cursor.
            UICheckBox zoomToCursorCheck = UICheckBoxes.AddLabelledCheckBox(this, Check1X, Check3Y, Translations.Translate("CAM_OPT_ZTC"));
            zoomToCursorCheck.isChecked = ModSettings.ZoomToCursor;
            zoomToCursorCheck.eventCheckChanged += (c, value) => { ModSettings.ZoomToCursor = value; };
            zoomToCursorCheck.tooltipBox = UIToolTips.WordWrapToolTip;
            zoomToCursorCheck.tooltip = Translations.Translate("CAM_OPT_ZTC_TIP");
        }

        /// <summary>
        /// Called by the game every update.
        /// Used to update slider values.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Update slider values to current camera target position.
            UpdateSliderValues();
        }

        /// <summary>
        /// Set panel visibility to the specified state.
        /// </summary>
        /// <param name="visible">True to show panel, false to hide.</param>
        internal static void SetState(bool visible)
        {
            if (visible)
            {
                StandalonePanelManager<CameraPanel>.Create();
            }
            else
            {
                StandalonePanelManager<CameraPanel>.Panel?.Close();
            }
        }

        /// <summary>
        /// Performs any actions required before closing the panel and checks that it's safe to do so.
        /// </summary>
        /// <returns>True if the panel can close now, false otherwise.</returns>
        protected override bool PreClose()
        {
            // Deselect UUI button.
            UUI.UUIButton.IsPressed = false;

            // Always okay to close.
            return true;
        }

        /// <summary>
        /// Target position slider value change event handler.
        /// </summary>
        /// <param name="c">Calling component.</param>
        /// <param name="value">New value.</param>
        private void PositionSliderChanged(UIComponent c, float value)
        {
            // Slider
            if (c is CameraSlider slider)
            {
                // Update tied value.
                switch (slider.name)
                {
                    case "xPos":
                        _controller.m_targetPosition.x = slider.value;
                        break;
                    case "zPos":
                        _controller.m_targetPosition.z = slider.value;
                        break;
                    case "rot":
                        _controller.m_targetAngle.x = slider.value;
                        break;
                    case "size":
                        _controller.m_targetSize = slider.value;
                        break;
                    case "tilt":
                        _controller.m_targetAngle.y = slider.value;
                        _controller.m_maxTiltDistance = 90f;
                        break;
                    case "fov":
                        _mainCamera.fieldOfView = Mathf.Clamp(slider.value, MinFOV, MaxFOV);
                        break;
                    case "shadMax":
                        CameraUtils.MaxShadowDistance = slider.value;
                        break;
                    case "shadMin":
                        CameraUtils.MinShadowDistance = slider.value;
                        break;
                }
            }
        }

        /// <summary>
        /// Updates slider values to current camera target position.
        /// </summary>
        private void UpdateSliderValues()
        {
            // Get camera controller reference.
            CameraController controller = CameraUtils.Controller;

            if (controller == null)
            {
                Logging.Error("unable to get camera controller reference");
                Close();
                return;
            }

            if (_xPosSlider == null || _zPosSlider == null || _rotSlider == null || _zoomSlider == null || _tiltSlider == null || _fovSlider == null)
            {
                Logging.Error("null slider reference");
                Close();
                return;
            }

            // Set slider values.
            _xPosSlider.value = controller.m_targetPosition.x;
            _zPosSlider.value = controller.m_targetPosition.z;
            _rotSlider.value = controller.m_targetAngle.x;
            _zoomSlider.value = controller.m_targetSize;
            _tiltSlider.value = controller.m_targetAngle.y;
            _fovSlider.value = _mainCamera.fieldOfView;
            _shadowMaxSlider.value = controller.m_maxShadowDistance;
            _shadowMinSlider.value = controller.m_minShadowDistance;
        }

        /// <summary>
        /// Adds a camera slider to the specified component.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="xPos">Relative X position.</param>
        /// <param name="yPos">Relative Y position.</param>
        /// <param name="width">Slider width.</param>
        /// <param name="labelKey">Text label translation key.</param>
        /// <param name="minValue">Minimum displayed value.</param>
        /// <param name="maxValue">Maximum displayed value.</param>
        /// <param name="stepSize">Default slider step size.</param>
        /// <param name="initialValue">Initial value.</param>
        /// <param name="stringFormat">Value display string format.</param>
        /// <param name="name">Slider name.</param>
        /// <returns>New CameraSlider.</returns>
        private CameraSlider AddCameraSlider(UIComponent parent, float xPos, float yPos, float width, string labelKey, float minValue, float maxValue, float stepSize, float initialValue, string stringFormat, string name)
        {
            // Layout constants.
            const float SliderY = 18f;
            const float ValueY = 3f;
            const float LabelY = -13f;
            const float SliderHeight = 18f;
            const float FloatTextFieldWidth = 70f;

            // Slider control.
            CameraSlider newSlider = parent.AddUIComponent<CameraSlider>();
            newSlider.size = new Vector2(width, SliderHeight);
            newSlider.relativePosition = new Vector2(xPos, yPos + SliderY);
            newSlider.name = name;

            // Tooltip.
            newSlider.tooltip = Translations.Translate("CAM_SLI_TIP");
            newSlider.tooltipBox.pivot = UIPivotPoint.TopLeft;

            // Value field - added to parent, not to slider, otherwise slider catches all input attempts.  Integer textfields (stepsize == 1) have shorter widths.
            float textFieldWidth = FloatTextFieldWidth;
            UITextField valueField = UITextFields.AddTinyTextField(parent, xPos + newSlider.width - textFieldWidth, yPos + ValueY, textFieldWidth);

            // Title label.
            UILabel titleLabel = UILabels.AddLabel(newSlider, 0f, LabelY, Translations.Translate(labelKey), textScale: 0.7f);

            // Autoscale tile label text, with minimum size 0.35.
            while (titleLabel.width > newSlider.width - textFieldWidth && titleLabel.textScale > 0.35f)
            {
                titleLabel.textScale -= 0.05f;
            }

            // Slider track.
            UISlicedSprite sliderSprite = newSlider.AddUIComponent<UISlicedSprite>();
            sliderSprite.atlas = UITextures.InGameAtlas;
            sliderSprite.spriteName = "BudgetSlider";
            sliderSprite.size = new Vector2(newSlider.width, 9f);
            sliderSprite.relativePosition = new Vector2(0f, 4f);

            // Slider thumb.
            UISlicedSprite sliderThumb = newSlider.AddUIComponent<UISlicedSprite>();
            sliderThumb.atlas = UITextures.InGameAtlas;
            sliderThumb.spriteName = "SliderBudget";
            newSlider.thumbObject = sliderThumb;

            // Set references.
            newSlider.ValueField = valueField;

            // Text display format.
            newSlider.StringFormat = stringFormat;

            // Event handler for textfield.
            newSlider.ValueField.eventTextSubmitted += newSlider.OnTextSubmitted;

            // Set initial values.
            newSlider.StepSize = stepSize;
            newSlider.maxValue = maxValue;
            newSlider.minValue = minValue;
            newSlider.value = initialValue;

            // Add value changed event handler.
            newSlider.eventValueChanged += PositionSliderChanged;

            return newSlider;
        }
    }
}
