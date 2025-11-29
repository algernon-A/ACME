// <copyright file="UIThreading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using AlgernonCommons.Keybinding;
    using ICities;
    using UnityEngine;

    /// <summary>
    /// Threading to capture hotkeys.
    /// </summary>
    public class UIThreading : ThreadingExtensionBase
    {
        /// <summary>
        /// Number of camera position slots.
        /// </summary>
        internal const int NumSlots = 22;

        // Instance reference.
        private static UIThreading s_instance;

        // Hotkeys.
        private static Keybinding s_moveItKey = new Keybinding(KeyCode.Mouse2, false, false, true);
        private static Keybinding s_fpsKey = new Keybinding(KeyCode.Tab, true, false, false);
        private static Keybinding s_resetKey = new Keybinding(KeyCode.R, false, false, true);

        // Fixed rotation keys.
        private static KeyOnlyBinding s_xKey = new KeyOnlyBinding(KeyCode.X);
        private static KeyOnlyBinding s_yKey = new KeyOnlyBinding(KeyCode.Y);

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
            KeyCode.F12,
        };

        // Flags.
        private bool _operating = false;
        private bool _moveItProcessed = false;
        private bool _fpsProcessed = false;
        private bool _resetProcessed = false;
        private bool _rotationProcessed = false;
        private int _positionKeyProcessed = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIThreading"/> class.
        /// </summary>
        public UIThreading()
        {
            // Set instance reference.
            s_instance = this;
        }

        /// <summary>
        /// Sets a value indicating whether hotkey detection is active.
        /// </summary>
        internal static bool Operating
        {
            set
            {
                if (s_instance != null)
                {
                    s_instance._operating = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the zoom to Move It selection hotkey.
        /// </summary>
        internal static Keybinding MoveItKey { get => s_moveItKey; set => s_moveItKey = value; }

        /// <summary>
        /// Gets or sets the zoom FPS activation hotkey.
        /// </summary>
        internal static Keybinding FPSModeKey { get => s_fpsKey; set => s_fpsKey = value; }

        /// <summary>
        /// Gets or sets the reset rotation hotkey.
        /// </summary>
        internal static Keybinding ResetKey { get => s_resetKey; set => s_resetKey = value; }

        /// <summary>
        /// Gets or sets the x-rotation hotkey.
        /// </summary>
        internal static KeyOnlyBinding XKey { get => s_xKey; set => s_xKey = value; }

        /// <summary>
        /// Gets or sets the y-rotation hotkey.
        /// </summary>
        internal static KeyOnlyBinding YKey { get => s_yKey; set => s_yKey = value; }

        /// <summary>
        /// Gets the key binding for loading saved camera positions.
        /// </summary>
        internal static Keybinding[] LoadKeyBindings { get; } = new Keybinding[NumSlots]
        {
             new Keybinding(KeyCode.Alpha0, true, false, false),
             new Keybinding(KeyCode.Alpha1, true, false, false),
             new Keybinding(KeyCode.Alpha2, true, false, false),
             new Keybinding(KeyCode.Alpha3, true, false, false),
             new Keybinding(KeyCode.Alpha4, true, false, false),
             new Keybinding(KeyCode.Alpha5, true, false, false),
             new Keybinding(KeyCode.Alpha6, true, false, false),
             new Keybinding(KeyCode.Alpha7, true, false, false),
             new Keybinding(KeyCode.Alpha8, true, false, false),
             new Keybinding(KeyCode.Alpha9, true, false, false),
             new Keybinding(KeyCode.F1, true, false, false),
             new Keybinding(KeyCode.F2, true, false, false),
             new Keybinding(KeyCode.F3, true, false, false),
             new Keybinding(KeyCode.F4, true, false, false),
             new Keybinding(KeyCode.F5, true, false, false),
             new Keybinding(KeyCode.F6, true, false, false),
             new Keybinding(KeyCode.F7, true, false, false),
             new Keybinding(KeyCode.F8, true, false, false),
             new Keybinding(KeyCode.F9, true, false, false),
             new Keybinding(KeyCode.F10, true, false, false),
             new Keybinding(KeyCode.F11, true, false, false),
             new Keybinding(KeyCode.F12, true, false, false),
        };

        /// <summary>
        /// Gets the key binding for saving camera positions.
        /// </summary>
        internal static Keybinding[] SaveKeyBindings { get; } = new Keybinding[NumSlots]
        {
             new Keybinding(KeyCode.Alpha0, true, true, false),
             new Keybinding(KeyCode.Alpha1, true, true, false),
             new Keybinding(KeyCode.Alpha2, true, true, false),
             new Keybinding(KeyCode.Alpha3, true, true, false),
             new Keybinding(KeyCode.Alpha4, true, true, false),
             new Keybinding(KeyCode.Alpha5, true, true, false),
             new Keybinding(KeyCode.Alpha6, true, true, false),
             new Keybinding(KeyCode.Alpha7, true, true, false),
             new Keybinding(KeyCode.Alpha8, true, true, false),
             new Keybinding(KeyCode.Alpha9, true, true, false),
             new Keybinding(KeyCode.F1, true, true, false),
             new Keybinding(KeyCode.F2, true, true, false),
             new Keybinding(KeyCode.F3, true, true, false),
             new Keybinding(KeyCode.F4, true, true, false),
             new Keybinding(KeyCode.F5, true, true, false),
             new Keybinding(KeyCode.F6, true, true, false),
             new Keybinding(KeyCode.F7, true, true, false),
             new Keybinding(KeyCode.F8, true, true, false),
             new Keybinding(KeyCode.F9, true, true, false),
             new Keybinding(KeyCode.F10, true, true, false),
             new Keybinding(KeyCode.F11, true, true, false),
             new Keybinding(KeyCode.F12, true, true, false),
        };

        /// <summary>
        /// Look for keypress to activate tool.
        /// </summary>
        /// <param name="realTimeDelta">Real-time delta since last update.</param>
        /// <param name="simulationTimeDelta">Simulation time delta since last update.</param>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Don't do anything if not active.
            if (_operating)
            {
                for (int i = 0; i < NumSlots; ++i)
                {
                    // Don't do anything if we're already processing this key.
                    if (_positionKeyProcessed == i)
                    {
                        continue;
                    }

                    // Check for saved position load key.
                    if (LoadKeyBindings[i].IsPressed())
                    {
                        // Process keystroke.
                        _positionKeyProcessed = i;
                        AlgernonCommons.Logging.Message($"UIThreading: loading position {i}");

                        // Load saved position.
                        CameraPositions.LoadPosition(i);
                        return;
                    }

                    // Check for position save key.
                    if (SaveKeyBindings[i].IsPressed())
                    {
                        // Process keystroke.
                        _positionKeyProcessed = i;

                        AlgernonCommons.Logging.Message($"UIThreading: saving position {i}");

                        // Load saved position.
                        CameraPositions.SavePosition(i);
                        return;
                    }
                }

                // If we got here, position keys aren't pressed.
                _positionKeyProcessed = -1;

                // Check for Movit zoom.
                if (s_moveItKey.IsPressed() && ToolsModifierControl.toolController.CurrentTool.GetType().ToString().Equals("MoveIt.MoveItTool"))
                {
                    // Only process if we're not already doing so.
                    if (!_moveItProcessed)
                    {
                        // Set processed flag.
                        _moveItProcessed = true;

                        // Set camera to Move It selection position.
                        MoveItUtils.SetPosition();
                    }
                }
                else
                {
                    // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                    _moveItProcessed = false;
                }

                // Check for FPS hotkey.
                if (s_fpsKey.IsPressed())
                {
                    // Only process if we're not already doing so.
                    if (!_fpsProcessed)
                    {
                        // Set processed flag.
                        _fpsProcessed = true;

                        // Toggle FPS mode.
                        FPSMode.ToggleMode();
                    }
                }
                else
                {
                    // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                    _fpsProcessed = false;
                }

                // Check for rotation reset hotkey.
                if (s_resetKey.IsPressed())
                {
                    // Only process if we're not already doing so.
                    if (!_resetProcessed)
                    {
                        // Set processed flag.
                        _resetProcessed = true;

                        // Toggle FPS mode.
                        CameraUtils.ResetRotation();
                    }
                }
                else
                {
                    // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                    _resetProcessed = false;
                }

                // Check for movement hotkeys (requires control down).
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    // Check modifiers.
                    bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr);

                    // Movement hotkeys.
                    if (s_xKey.IsPressed())
                    {
                        // Only process if we're not already doing so.
                        if (!_rotationProcessed)
                        {
                            // Set processed flag.
                            _rotationProcessed = true;

                            // Rotate camera.
                            CameraUtils.RotateX(altPressed ? 45f : 15f, shiftPressed);
                        }
                    }
                    else if (s_yKey.IsPressed())
                    {
                        // Only process if we're not already doing so.
                        if (!_rotationProcessed)
                        {
                            // Set processed flag.
                            _rotationProcessed = true;

                            // Rotate camera.
                            CameraUtils.RotateY(altPressed ? 45f : 15f, shiftPressed);
                        }
                    }
                    else
                    {
                        // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                        _rotationProcessed = false;
                    }
                }
                else
                {
                    // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                    _rotationProcessed = false;
                }
            }
        }
    }
}