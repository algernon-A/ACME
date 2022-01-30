using System;
using ColossalFramework.UI;
using UnityEngine;


namespace ACME
{
	/// <summary>
	/// Static class to manage the BOB info panel.
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
		private const float Check1Y = FovSliderY + SliderHeight;
		private const float Check2Y = Check1Y + CheckHeight;
		private const float Check1X = Margin;
		private const float Check2X = (PanelWidth - (Margin * 2f)) / 2f;
		private const float PanelHeight = Check2Y + CheckHeight + Margin;

		// Camera constants.
		private const float MinFOV = 24f;
		private const float MaxFOV = 84f;


		// Camera components.
		private readonly CameraSlider xPosSlider, zPosSlider, rotSlider, zoomSlider, tiltSlider, fovSlider;


		// Instance references.
		private static GameObject uiGameObject;
		private static CameraPanel panel;
		internal static CameraPanel Panel => panel;
		private static CameraController controller;



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

			if (xPosSlider == null || zPosSlider == null || rotSlider == null || zoomSlider == null || tiltSlider == null || fovSlider == null)
            {
				Logging.Error("null slider reference");
				Close();
				return;
            }

			// Set slider values.
			xPosSlider.value = controller.m_targetPosition.x;
			zPosSlider.value = controller.m_targetPosition.z;
			rotSlider.value = controller.m_targetAngle.x;
			zoomSlider.value = controller.m_targetSize;
			tiltSlider.value = controller.m_targetAngle.y;

			/*Camera mainCam = Camera.main;

			if (mainCam == null)
            {
				Logging.Error("unable to get main camera reference");
				Close();
				return;
			}
			fovSlider.value = Camera.main.fieldOfView;*/
		}


        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        internal static void Create()
		{
			try
			{
				// If no GameObject instance already set, create one.
				if (uiGameObject == null)
				{
					// Give it a unique name for easy finding with ModTools.
					uiGameObject = new GameObject("ACMECameraPanel");
					uiGameObject.transform.parent = UIView.GetAView().transform;

					// Create new panel instance and add it to GameObject.
					panel = uiGameObject.AddComponent<CameraPanel>();
					panel.transform.parent = uiGameObject.transform.parent;
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
			if (panel == null)
			{
				return;
			}

			// Destroy game objects.
			GameObject.Destroy(panel);
			GameObject.Destroy(uiGameObject);

			// Let the garbage collector do its work (and also let us know that we've closed the object).
			panel = null;
			uiGameObject = null;

			// Unpress UUI button.
			UUI.UUIButton.IsPressed = false;
		}


		/// <summary>
		/// <summary>
		/// Set panel visibility to the specified state.
		/// </summary>
		/// <param name="visible">True to show panel, false to hide</param>
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
		/// Constructor.
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

			Logging.Message("adding basic components");

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

			Logging.Message("getting camera controller");

			// Get camera controller reference.
			controller = CameraUtils.Controller;

			Logging.Message("setting up sliders");

			// X-position slider.
			xPosSlider = AddCameraSlider(this, Margin, XSliderY, PanelWidth - (Margin * 2f), "CAM_XPOS", -8500f, 8500f, 0.01f, controller.m_targetPosition.x, "N1", "xPos");

			// Z-position slider.
			zPosSlider = AddCameraSlider(this, Margin, ZSliderY, PanelWidth - (Margin * 2f), "CAM_ZPOS", -8500f, 8500f, 0.01f, controller.m_targetPosition.z, "N1", "zPos");

			// Rotation around target slider.
			rotSlider = AddCameraSlider(this, Margin, RotSliderY, PanelWidth - (Margin * 2f), "CAM_ROT", -180f, 180f, 0.01f, controller.m_targetAngle.x, "N2", "rot");

			// Zoom slider.
			zoomSlider = AddCameraSlider(this, Margin, ZoomSliderY, PanelWidth - (Margin * 2f), "CAM_SIZE", controller.m_minDistance, controller.m_maxDistance, 1f, controller.m_targetSize, "N1", "size");

			// Tilt slider.
			tiltSlider = AddCameraSlider(this, Margin, TiltSliderY, PanelWidth - (Margin * 2f), "CAM_TILT", -90f, 90f, 0.01f, controller.m_targetAngle.y, "N2", "tilt");

			// FOV slider.
			fovSlider = AddCameraSlider(this, Margin, FovSliderY, PanelWidth - (Margin * 2f), "CAM_FOV", MinFOV, MaxFOV, 0.01f, CameraUtils.MainCamera.fieldOfView, "N1", "fov");

			Logging.Message("setting up checkboxes");

			// Building collision checkbox.
			UICheckBox buildingCollisionCheck = UIControls.LabelledCheckBox(this, Check1X, Check1Y, Translations.Translate("CAM_COL_BLD"));
			buildingCollisionCheck.isChecked = HeightOffset.buildingCollision;
			buildingCollisionCheck.eventCheckChanged += (control, value) => { HeightOffset.buildingCollision = value; };

			// Network collision checkbox.
			UICheckBox netCollisionCHeck = UIControls.LabelledCheckBox(this, Check1X, Check2Y, Translations.Translate("CAM_COL_NET"));
			netCollisionCHeck.isChecked = HeightOffset.networkCollision;
			netCollisionCHeck.eventCheckChanged += (control, value) => { HeightOffset.networkCollision = value; };

			// Prop collision checkbox.
			UICheckBox propCollisionCHeck = UIControls.LabelledCheckBox(this, Check2X, Check1Y, Translations.Translate("CAM_COL_PRO"));
			propCollisionCHeck.isChecked = HeightOffset.propCollision;
			propCollisionCHeck.eventCheckChanged += (control, value) => { HeightOffset.propCollision = value; };

			// Tree collision checkbox.
			UICheckBox treeCollisionCheck = UIControls.LabelledCheckBox(this, Check2X, Check2Y, Translations.Translate("CAM_COL_TRE"));
			treeCollisionCheck.isChecked = HeightOffset.treeCollision;
			treeCollisionCheck.eventCheckChanged += (control, value) => { HeightOffset.treeCollision = value; };
		}


		/// <summary>
		/// Target position slider value change event handler.
		/// </summary>
		/// <param name="control">Calling component</param>
		/// <param name="value">New value</param>
		private void PositionSliderChanged(UIComponent control, float value)
		{
			// Slider
			if (control is CameraSlider slider)
			{
				// Update tied value.
				switch (slider.name)
				{
					case "xPos":
						controller.m_targetPosition.x = slider.value;
						break;
					case "zPos":
						controller.m_targetPosition.z = slider.value;
						break;
					case "rot":
						controller.m_targetAngle.x = slider.value;
						break;
					case "size":
						controller.m_targetSize = slider.value;
						break;
					case "tilt":
						controller.m_targetAngle.y = slider.value;
						controller.m_maxTiltDistance = 90f;
						break;
					case "fov":
						Logging.Message("fov slider changed");
						Camera.main.fieldOfView = Mathf.Clamp(MinFOV, slider.value, MaxFOV);
						break;
				}
			}
		}


		/// <summary>
		/// Adds a camera slider to the specified component.
		/// </summary>
		/// <param name="parent">Parent component</param>
		/// <param name="xPos">Relative X position</param
		/// <param name="yPos">Relative Y position</param
		/// <param name="width">Slider width</param>
		/// <param name="labelKey">Text label translation key</param>
		/// <param name="minValue">Minimum displayed value</param
		/// <param name="maxValue">Maximum displayed value</param>
		/// <param name="stepSize">Default slider step size</param>
		/// <param name="initialValue">Initial value</param>
		/// <param name="stringFormat">Value display string format</param>
		/// <param name="name">Slider name</param>
		/// <returns>New CameraSlider</returns>
		private CameraSlider AddCameraSlider(UIComponent parent, float xPos, float yPos, float width, string labelKey, float minValue, float maxValue, float stepSize, float initialValue, string stringFormat, string name)
		{
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

			// Value field - added to parent, not to slider, otherwise slider catches all input attempts.  Integer textfields (stepsize == 1) have shorter widths.
			float textFieldWidth = FloatTextFieldWidth;
			UITextField valueField = UIControls.TinyTextField(parent, xPos + newSlider.width - textFieldWidth, yPos + ValueY, textFieldWidth);

			// Title label.
			UILabel titleLabel = UIControls.AddLabel(newSlider, 0f, LabelY, Translations.Translate(labelKey), textScale: 0.7f);

			// Autoscale tile label text, with minimum size 0.35.
			while (titleLabel.width > newSlider.width - textFieldWidth && titleLabel.textScale > 0.35f)
			{
				titleLabel.textScale -= 0.05f;
			}

			// Slider track.
			UISlicedSprite sliderSprite = newSlider.AddUIComponent<UISlicedSprite>();
			sliderSprite.atlas = TextureUtils.InGameAtlas;
			sliderSprite.spriteName = "BudgetSlider";
			sliderSprite.size = new Vector2(newSlider.width, 9f);
			sliderSprite.relativePosition = new Vector2(0f, 4f);

			// Slider thumb.
			UISlicedSprite sliderThumb = newSlider.AddUIComponent<UISlicedSprite>();
			sliderThumb.atlas = TextureUtils.InGameAtlas;
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
