using UnityEngine;
using ColossalFramework.UI;


namespace ACME
{
	/// <summary>
	/// Slider with integrated components.
	/// </summary>
	public class CameraSlider : UISlider
	{
		// State flag (to avoid infinite recursive update loops).
		private bool suppressEvents = false;

		// Value field display format.
		public string StringFormat { get; set; } = "N0";

		// Limit to visible range?
		public bool LimitToVisible { get; set; } = false;


		// Sub-components.
		public UITextField ValueField { get; set; }



		/// <summary>
		/// Minimum slider step size; underlying step size is 1/10th of value, to ensure small changes aren't quantized out.
		/// </summary>
		public float StepSize { set => stepSize = value / 10f; }


		/// <summary>
		/// Handles textfield value change; should be added as eventTextSubmitted event handler.
		/// </summary>
		/// <param name="control">Calling component(unused)</param>
		/// <param name="text">New text</param>
		public void OnTextSubmitted(UIComponent _, string text)
		{
			// Don't do anything is events are suppressed.
			if (!suppressEvents)
			{
				// Suppress events while we change things, to avoid infinite recursive update loops.
				suppressEvents = true;

				// Attempt to parse textfield value.
				if (float.TryParse(text, out float result))
				{
					// Successful parse - set slider value.
					value = result;
				}

				// Restore event handling.
				suppressEvents = false;
			}
		}


		/// <summary>
		/// Called by game when slider value is changed.
		/// </summary>
		protected override void OnValueChanged()
		{
			// Update field text.
			ValueField.text = value.ToString(StringFormat);

			// Complete normal slider value change processing (update thumb position, invoke events, etc.).
			base.OnValueChanged();
		}


		/// <summary>
		/// Called by game when mousewheel is scrolled.
		/// </summary>
		/// <param name="mouseEvent">Mouse event parameter</param>
		protected override void OnMouseWheel(UIMouseEventParameter mouseEvent)
		{
			// Set current value according to multiplier state, suppressing events first to avoid value clamping, and manually updating textfield.
			suppressEvents = true;

			// Multiply mouse wheel movement by current step multiplier.
			value = value + (mouseEvent.wheelDelta * WheelMultiplier);
			suppressEvents = false;

			// Use event and invoke any handlers.
			mouseEvent.Use();
			Invoke("OnMouseWheel", mouseEvent);
		}


		/// <summary>
		/// Returns the current mouse wheel step multiplier based on modifier key states.
		/// 100/10/1.0/0.1f on shift/none/alt/ctrl.
		/// </summary>
		private float WheelMultiplier
		{
			get
			{
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					// Shift modifier.
					return 100f;
				}
				else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr))
				{
					// Alt modifier.
					return 1f;
				}
				else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
				{
					// Control modifier.
					return 0.1f;
				}
				else
				{
					// Default multiplier.
					return 10f;
				}
			}
		}
	}
}