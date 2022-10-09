// <copyright file="CameraPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System;
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Static class to manage the ACME camera panel.
    /// </summary>
    internal class CameraPanel : UIPanel
    {
        // Layout constants - general.
        private const float Margin = 5f;

        // Layout constants - X.
        private const float PanelWidth = 400f;

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
        private const float Check2X = (PanelWidth - (Margin * 2f)) / 2f;
        private const float PanelHeight = Check3Y + CheckHeight + Margin;

        // Camera constants.
        private const float MinFOV = 5f;
        private const float MaxFOV = 84f;

        // Instance references.
        private static GameObject s_gameObject;
        private static CameraPanel s_panel;

        // Camera references.
        private readonly Camera _mainCamera;
        private readonly CameraController _controller;

        // Panel components.
        private readonly CameraSlider _xPosSlider;
        private readonly CameraSlider _zPosSlider;
        private readonly CameraSlider _rotSlider;
        private readonly CameraSlider _zoomSlider;
        private readonly CameraSlider _tiltSlider;
        private readonly CameraSlider _fovSlider;
        private readonly CameraSlider _shadowMaxSlider;
        private readonly CameraSlider _shadowMinSlider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPanel"/> class.
        /// </summary>
        internal CameraPanel()
        {
            // Basic behaviour.
            autoLayout = false;
            canFocus = true;
            isInteractive = true;

            // Appearance.
            backgroundSprite = "MenuPanel2";
            opacity = 1f;

            // Size.
            size = new Vector2(PanelWidth, PanelHeight);

            // Default position - centre in screen.
            relativePosition = new Vector2(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));

            // Drag bar.
            UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.width = this.width - 50f;
            dragHandle.height = this.height;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            // Title label.
            UILabel titleLabel = AddUIComponent<UILabel>();
            titleLabel.relativePosition = new Vector2(50f, 13f);
            titleLabel.text = Translations.Translate("CAM_NAM");

            // Close button.
            UIButton closeButton = AddUIComponent<UIButton>();
            closeButton.relativePosition = new Vector2(width - 35, 2);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.eventClick += (component, clickEvent) => Close();

            // Decorative icon (top-left).
            UISprite iconSprite = AddUIComponent<UISprite>();
            iconSprite.relativePosition = new Vector2(5, 5);
            iconSprite.height = 32f;
            iconSprite.width = 32f;
            iconSprite.atlas = UITextures.LoadSingleSpriteAtlas("ACME-UUI");
            iconSprite.spriteName = "normal";

            // Set camera references.
            _controller = CameraUtils.Controller;
            _mainCamera = CameraUtils.MainCamera;

            // X-position slider.
            _xPosSlider = AddCameraSlider(this, Margin, XSliderY, PanelWidth - (Margin * 2f), "CAM_XPOS", -8500f, 8500f, 0.01f, _controller.m_targetPosition.x, "N1", "xPos");

            // Z-position slider.
            _zPosSlider = AddCameraSlider(this, Margin, ZSliderY, PanelWidth - (Margin * 2f), "CAM_ZPOS", -8500f, 8500f, 0.01f, _controller.m_targetPosition.z, "N1", "zPos");

            // Rotation around target slider.
            _rotSlider = AddCameraSlider(this, Margin, RotSliderY, PanelWidth - (Margin * 2f), "CAM_ROT", -180f, 180f, 0.01f, _controller.m_targetAngle.x, "N2", "rot");

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
        /// Gets the active panel instance.
        /// </summary>
        internal static CameraPanel Panel => s_panel;

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
        /// Creates the panel object in-game and displays it.
        /// </summary>
        internal static void Create()
        {
            try
            {
                // If no GameObject instance already set, create one.
                if (s_gameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    s_gameObject = new GameObject("ACMECameraPanel");
                    s_gameObject.transform.parent = UIView.GetAView().transform;

                    // Create new panel instance and add it to GameObject.
                    s_panel = s_gameObject.AddComponent<CameraPanel>();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception creating search panel");
            }

            // Press UUI button.
            UUI.UUIButton.IsPressed = true;
        }

        /// <summary>
        /// Closes the panel by destroying the object (removing any ongoing UI overhead).
        /// </summary>
        internal static void Close()
        {
            // Don't do anything if no panel.
            if (s_panel == null)
            {
                return;
            }

            // Destroy game objects.
            Destroy(s_panel);
            Destroy(s_gameObject);

            // Let the garbage collector do its work (and also let us know that we've closed the object).
            s_panel = null;
            s_gameObject = null;

            // Unpress UUI button.
            UUI.UUIButton.IsPressed = false;
        }

        /// <summary>
        /// Set panel visibility to the specified state.
        /// </summary>
        /// <param name="visible">True to show panel, false to hide.</param>
        internal static void SetState(bool visible)
        {
            if (visible)
            {
                Create();
            }
            else
            {
                Close();
            }
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
