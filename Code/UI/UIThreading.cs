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
        // Instance reference.
        private static UIThreading s_instance;

        // Hotkeys.
        private static Keybinding s_moveItKey = new Keybinding(KeyCode.Mouse2, false, false, true);
        private static Keybinding s_fpsKey = new Keybinding(KeyCode.Tab, true, false, false);
        private static Keybinding s_resetKey = new Keybinding(KeyCode.R, false, false, true);

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
        /// Look for keypress to activate tool.
        /// </summary>
        /// <param name="realTimeDelta">Real-time delta since last update.</param>
        /// <param name="simulationTimeDelta">Simulation time delta since last update.</param>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Don't do anything if not active.
            if (_operating)
            {
                // Check modifier keys according to settings.
                bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                // Save/restore position - is control pressed?
                if (ctrlPressed)
                {
                    // Is a saved position key also pressed?
                    for (int i = 0; i < positionKeys.Length; ++i)
                    {
                        // Don't do anything if we're already processing this key.
                        if (_positionKeyProcessed == i)
                        {
                            continue;
                        }

                        if (Input.GetKey(positionKeys[i]))
                        {
                            // Saved position key pressed.
                            _positionKeyProcessed = i;

                            // Loading or saving (shift key)?
                            if (shiftPressed)
                            {
                                // Shift is pressed - saving.
                                CameraPositions.SavePosition(i);
                                return;
                            }
                            else
                            {
                                // Shift is not pressed - loading.
                                CameraPositions.LoadPosition(i);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                    _positionKeyProcessed = -1;
                }

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
                    bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr);

                    // Movement hotkeys.
                    if (Input.GetKey(KeyCode.X))
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
                    else if (Input.GetKey(KeyCode.Y))
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