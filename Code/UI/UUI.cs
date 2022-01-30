using UnityEngine;
using ColossalFramework;
using UnifiedUI.Helpers;


namespace ACME
{
	/// <summary>
	/// Static class to handle UUI interface.
	/// </summary>
    internal static class UUI
	{
		// UUI Button.
		private static UUICustomButton uuiButton;

		// SavedInputKey reference for communicating with UUI.
		internal static readonly SavedInputKey uuiSavedKey = new SavedInputKey("ACME hotkey", "ACME hotkey", key: KeyCode.C, control: false, shift: false, alt: true, false);


		/// <summary>
		/// UUI button accessor.
		/// </summary>S
		internal static UUICustomButton UUIButton => uuiButton;


		/// <summary>
		/// Performs initial setup and creates the UUI button.
		/// </summary>
		internal static void Setup()
		{
			// Add UUI button.
			if (uuiButton == null)
			{
				uuiButton = UUIHelpers.RegisterCustomButton(
					name: ACMEMod.ModName,
					groupName: null, // default group
					tooltip: Translations.Translate("CAM_NAM"),
					icon: UUIHelpers.LoadTexture(UUIHelpers.GetFullPath<ACMEMod>("Resources", "ACME-UUI.png")),
					onToggle: (value) => CameraPanel.SetState(value),
					hotkeys: new UUIHotKeys { ActivationKey = ModSettings.ToggleSavedKey }
					);

				// Set initial state.
				uuiButton.IsPressed = false;
			}
		}
	}
}