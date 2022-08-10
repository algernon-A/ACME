// <copyright file="CameraPositions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System.IO;
    using AlgernonCommons;
    using UnityEngine;

    /// <summary>
    /// Utility class for changing camera settings.
    /// </summary>
    public static class CameraPositions
    {
        // Numer of stored positions available.
        internal const int NumSaves = 22;

        // Array of saved positions.
        private static SavedPosition[] savedPositions = new SavedPosition[NumSaves];

        /// <summary>
        /// Saves a camera position.
        /// </summary>
        /// <param name="positionIndex">Camera position index to save to</param>
        internal static void SavePosition(int positionIndex)
        {
            // Save current camera attributes.
            savedPositions[positionIndex] = CurrentPosition();
        }

        /// <summary>
        /// Loads a saved camera position.
        /// </summary>
        /// <param name="positionIndex">Camera position index to load from</param>
        internal static void LoadPosition(int positionIndex)
        {
            // Don't do anything if position isn't valid.
            if (savedPositions[positionIndex].IsValid)
            {
                // Local reference.
                CameraController controller = CameraUtils.Controller;

                // Restore saved attributes.
                controller.m_targetPosition = savedPositions[positionIndex].Position;
                controller.m_currentPosition = savedPositions[positionIndex].Position;
                controller.m_targetAngle = savedPositions[positionIndex].Angle;
                controller.m_currentAngle = savedPositions[positionIndex].Angle;
                controller.m_targetHeight = savedPositions[positionIndex].Height;
                controller.m_currentHeight = savedPositions[positionIndex].Height;
                controller.m_targetSize = savedPositions[positionIndex].Size;
                controller.m_currentSize = savedPositions[positionIndex].Size;
                CameraUtils.MainCamera.fieldOfView = savedPositions[positionIndex].FOV;
            }
        }

        /// <summary>
        /// Serializes savegame data.
        /// </summary>
        /// <param name="writer">BinaryWriter instance to serialize to.</param>
        internal static void Serialize(BinaryWriter writer)
        {
            Logging.Message("serializing camera positions");

            // Write version.
            writer.Write(1);

            // Serialise each position.
            for (int i = 0; i < NumSaves; ++i)
            {
                // Serialize position.
                WritePosition(savedPositions[i], writer);
            }

            // Serialize current position.
            WritePosition(CurrentPosition(), writer);
        }

        /// <summary>
        /// Deserializes savegame data.
        /// </summary>
        /// <param name="reader">BinaryReader instance to deserialize from.</param>
        internal static void Deserialize(BinaryReader reader)
        {
            Logging.Message("deserializing savegame data");

            // Read version.
            int version = reader.ReadInt32();

            // Serialise each position.
            for (int i = 0; i < NumSaves; ++i)
            {
                // Deserialize position.
                savedPositions[i] = ReadPosition(reader);
            }

            // If version 1, read current camera position.
            if (version == 1)
            {
                CameraUtils.initialPosition = ReadPosition(reader);
            }
        }

        /// <summary>
        /// Gets current camera position as a SavedPosition struct.
        /// </summary>
        /// <returns>New SavedPosition struct reflecting current camera position.</returns>
        private static SavedPosition CurrentPosition()
        {
            // Local reference.
            CameraController controller = CameraUtils.Controller;

            // Save current camera attributes.
            return new SavedPosition
            {
                IsValid = true,
                Position = controller.m_targetPosition,
                Angle = controller.m_targetAngle,
                Height = controller.m_targetHeight,
                Size = controller.m_currentSize,
                FOV = CameraUtils.MainCamera.fieldOfView
            };
        }

        /// <summary>
        /// Serializes a camera position to the given BinaryWriter.
        /// </summary>
        /// <param name="position">Position to serialize</param>
        /// <param name="writer">Target BinaryWriter</param>
        private static void WritePosition(SavedPosition position, BinaryWriter writer)
        {
            writer.Write(position.IsValid);
            writer.Write(position.Position.x);
            writer.Write(position.Position.y);
            writer.Write(position.Position.z);
            writer.Write(position.Angle.x);
            writer.Write(position.Angle.y);
            writer.Write(position.Height);
            writer.Write(position.Size);
            writer.Write(position.FOV);
        }

        /// <summary>
        /// Deserializes a camera position from the given BinaryReader.
        /// </summary>
        /// <param name="reader">BinaryReader to use</param>
        /// <returns>New saved camera position</returns>
        private static SavedPosition ReadPosition(BinaryReader reader)
        {
            return new SavedPosition
            {
                // Serialize key and simple fields.
                IsValid = reader.ReadBoolean(),
                Position = new Vector3
                {
                    x = reader.ReadSingle(),
                    y = reader.ReadSingle(),
                    z = reader.ReadSingle()
                },
                Angle = new Vector2
                {
                    x = reader.ReadSingle(),
                    y = reader.ReadSingle()
                },
                Height = reader.ReadSingle(),
                Size = reader.ReadSingle(),
                FOV = reader.ReadSingle()
            };
        }

        /// <summary>
        /// Structure to store saved camera positions.
        /// </summary>
        public struct SavedPosition
        {
            /// <summary>
            /// Indicates whether or not this position record is valid (in-use).
            /// </summary>
            public bool IsValid;

            /// <summary>
            /// Camera position.
            /// </summary>
            public Vector3 Position;

            /// <summary>
            /// Camera angle.
            /// </summary>
            public Vector2 Angle;

            /// <summary>
            /// Camera height.
            /// </summary>
            public float Height;

            /// <summary>
            /// Camera size.
            /// </summary>
            public float Size;

            /// <summary>
            /// Camera Field Of View.
            /// </summary>
            public float FOV;
        }
    }
}