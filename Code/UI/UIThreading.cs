using UnityEngine;
using ICities;


namespace ACME
{
    /// <summary>
    /// Threading to capture hotkeys.
    /// </summary>
    public class UIThreading : ThreadingExtensionBase
    {
        // Instance reference.
        private static UIThreading instance;

        // Flags.
        private bool operating = false;
        private bool moveItProcessed = false;
        private bool fpsProcessed = false;

        // Saved position hotkeys.
        private readonly KeyCode[] positionKeys = new KeyCode[CameraPositions.NumSaves]
        {
            KeyCode.Alpha0,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.F1,
            KeyCode.F2,
            KeyCode.F3,
            KeyCode.F4,
            KeyCode.F5,
            KeyCode.F6,
            KeyCode.F7,
            KeyCode.F8,
            KeyCode.F9,
            KeyCode.F10,
            KeyCode.F11,
            KeyCode.F12
        };


        // Hotkeys.
        internal static KeyBinding moveItKey = new KeyBinding(KeyCode.Mouse2, false, false, false);
        internal static KeyBinding fpsKey = new KeyBinding(KeyCode.Tab, true, false, false);


        /// <summary>
        /// Constructor - sets instance reference.
        /// </summary>
        public UIThreading()
        {
            // Set instance reference.
            instance = this;
        }


        /// <summary>
        /// Activates/deactivates hotkey.
        /// </summary>
        internal static bool Operating
        {
            set
            {
                if (instance != null)
                {
                    instance.operating = value;
                }
            }
        }


        /// <summary>
        /// Look for keypress to activate tool.
        /// </summary>
        /// <param name="realTimeDelta"></param>
        /// <param name="simulationTimeDelta"></param>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Don't do anything if not active.
            if (operating)
            {
                // Check modifier keys according to settings.
                bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr);
                bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                // Save/restore position - is control pressed?
                if (ctrlPressed)
                {
                    // Is a saved position key also pressed?
                    for (int i = 0; i < positionKeys.Length; ++i)
                    {
                        if (Input.GetKey(positionKeys[i]))
                        {
                            // Saved position key pressed - loading or saving (shift key)?
                            if (shiftPressed)
                            {
                                // Shift is pressed - saving.
                                CameraPositions.SavePosition(i);
                            }
                            else
                            {
                                // Shift is not pressed - loading.
                                CameraPositions.LoadPosition(i);
                            }
                        }
                    }
                }

                // Check for Movit zoom.
                // Modifiers have to *exactly match* settings, e.g. "alt-E" should not trigger on "ctrl-alt-E".
                bool altOkay = altPressed == moveItKey.alt;
                bool ctrlOkay = ctrlPressed == moveItKey.control;
                bool shiftOkay = shiftPressed == moveItKey.shift;
                bool modifiersOkay = altOkay & ctrlOkay & shiftOkay;
                bool keyPressed = modifiersOkay & Input.GetKey(moveItKey.key);
                if (keyPressed && ToolsModifierControl.toolController.CurrentTool.GetType().ToString().Equals("MoveIt.MoveItTool"))
                {
                    // Only process if we're not already doing so.
                    if (!moveItProcessed)
                    {
                        // Set processed flag.
                        moveItProcessed = true;

                        // Set camera to Move It selection position.
                        MoveItUtils.SetPosition();
                    }
                }
                else
                {
                    // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                    moveItProcessed = false;
                }

                // Check for FPS hotkey.
                // Modifiers have to *exactly match* settings, e.g. "alt-E" should not trigger on "ctrl-alt-E".
                altOkay = altPressed == fpsKey.alt;
                ctrlOkay = ctrlPressed == fpsKey.control;
                shiftOkay = shiftPressed == fpsKey.shift;
                modifiersOkay = altOkay & ctrlOkay & shiftOkay;
                if (modifiersOkay & Input.GetKey(fpsKey.key))
                {
                    // Only process if we're not already doing so.
                    if (!fpsProcessed)
                    {
                        // Set processed flag.
                        fpsProcessed = true;

                        // Toggle FPS mode.
                        FPSMode.ToggleMode();
                    }
                }
                else
                {
                    // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                    fpsProcessed = false;
                }
            }
        }
    }
}