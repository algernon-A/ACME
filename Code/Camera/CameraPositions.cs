// <copyright file="CameraPositions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using AlgernonCommons;
    using ColossalFramework;
    using ICities;
    using UnityEngine;

    /// <summary>
    /// Utility class for changing camera settings.
    /// </summary>
    public static class CameraPositions
    {
        /// <summary>
        /// Number of stored positions available.
        /// </summary>
        internal const int NumSaves = 22;

        // Array of saved positions.
        private static readonly SavedPosition[] GameSavedPositions = new SavedPosition[NumSaves];
        private static readonly SavedPosition[] EditorSavedPositions = new SavedPosition[NumSaves];

        /// <summary>
        /// Saves a camera position.
        /// </summary>
        /// <param name="positionIndex">Camera position index to save to.</param>
        internal static void SavePosition(int positionIndex)
        {
            // Save current camera attributes.
            if (Singleton<ToolManager>.instance.m_properties.m_mode == ItemClass.Availability.Game)
            {
                GameSavedPositions[positionIndex] = CurrentPosition();
            }
            else
            {
                // Editor positiosn are saved in the XML settings file.
                EditorSavedPositions[positionIndex] = CurrentPosition();
                ModSettings.Save();
            }

            // Play sound.
            Singleton<AudioManager>.instance.PlaySound(SoundEffects.SaveSound, 1f);
        }

        /// <summary>
        /// Loads a saved camera position.
        /// </summary>
        /// <param name="positionIndex">Camera position index to load from.</param>
        internal static void LoadPosition(int positionIndex)
        {
            // Get relevant position.
            SavedPosition savedPosition = Singleton<ToolManager>.instance.m_properties.m_mode == ItemClass.Availability.Game ? GameSavedPositions[positionIndex] : EditorSavedPositions[positionIndex];

            // Don't do anything if position isn't valid.
            if (savedPosition.IsValid)
            {
                // Local reference.
                CameraController controller = CameraUtils.Controller;

                // Restore saved attributes.
                controller.m_targetPosition = savedPosition.Position;
                controller.m_currentPosition = savedPosition.Position;
                controller.m_targetAngle = savedPosition.Angle;
                controller.m_currentAngle = savedPosition.Angle;
                controller.m_targetHeight = savedPosition.Height;
                controller.m_currentHeight = savedPosition.Height;
                controller.m_targetSize = savedPosition.Size;
                controller.m_currentSize = savedPosition.Size;
                CameraUtils.MainCamera.fieldOfView = savedPosition.FOV;
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
                WritePosition(GameSavedPositions[i], writer);
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
                GameSavedPositions[i] = ReadPosition(reader);
            }

            // If version 1, read current camera position.
            if (version == 1)
            {
                CameraUtils.InitialPosition = ReadPosition(reader);
            }
        }

        /// <summary>
        /// Serializes saved editor camera positions to XML.
        /// </summary>
        /// <returns>New array of XML-serialized camera positions.</returns>
        internal static SerializedPosition[] XMLSerialize()
        {
            // Serialize positions.
            List<SerializedPosition> postions = new List<SerializedPosition>(EditorSavedPositions.Length);
            for (int i = 0; i < EditorSavedPositions.Length; ++i)
            {
                // Only add valid positions.
                if (EditorSavedPositions[i].IsValid)
                {
                    postions.Add(new SerializedPosition
                    {
                        Index = i,
                        PosX = EditorSavedPositions[i].Position.x,
                        PosY = EditorSavedPositions[i].Position.y,
                        PosZ = EditorSavedPositions[i].Position.z,
                        AngleX = EditorSavedPositions[i].Angle.x,
                        AngleY = EditorSavedPositions[i].Angle.y,
                        Height = EditorSavedPositions[i].Height,
                        Size = EditorSavedPositions[i].Size,
                        FOV = EditorSavedPositions[i].FOV,
                    });
                }
            }

            return postions.ToArray();
        }

        /// <summary>
        /// Deserializes saved editor camera positions from XML.
        /// </summary>
        /// <param name="positions">List of XML-serialized camera positions to deserialize.</param>
        internal static void XMLDeserialize(SerializedPosition[] positions)
        {
            foreach (SerializedPosition position in positions)
            {
                EditorSavedPositions[position.Index].IsValid = true;
                EditorSavedPositions[position.Index].Position = new Vector3(position.PosX, position.PosY, position.PosZ);
                EditorSavedPositions[position.Index].Angle = new Vector2(position.AngleX, position.AngleY);
                EditorSavedPositions[position.Index].Height = position.Height;
                EditorSavedPositions[position.Index].Size = position.Size;
                EditorSavedPositions[position.Index].FOV = position.FOV;
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
                FOV = CameraUtils.MainCamera.fieldOfView,
            };
        }

        /// <summary>
        /// Serializes a camera position to the given BinaryWriter.
        /// </summary>
        /// <param name="position">Position to serialize.</param>
        /// <param name="writer">Target BinaryWriter.</param>
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
        /// <param name="reader">BinaryReader to use.</param>
        /// <returns>New saved camera position.</returns>
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
                    z = reader.ReadSingle(),
                },
                Angle = new Vector2
                {
                    x = reader.ReadSingle(),
                    y = reader.ReadSingle(),
                },
                Height = reader.ReadSingle(),
                Size = reader.ReadSingle(),
                FOV = reader.ReadSingle(),
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

        /// <summary>
        /// Structure to store saved camera positions in XML.
        /// </summary>
        [XmlRoot("SerializedPosition")]
        public struct SerializedPosition
        {
            /// <summary>
            /// Position index number.
            /// </summary>
            [XmlAttribute]
            public int Index;

            /// <summary>
            /// Camera X-position.
            /// </summary>
            [XmlAttribute]
            public float PosX;

            /// <summary>
            /// Camera Y-position.
            /// </summary>
            [XmlAttribute]
            public float PosY;

            /// <summary>
            /// Camera Z-position.
            /// </summary>
            [XmlAttribute]
            public float PosZ;

            /// <summary>
            /// Camera X-angle.
            /// </summary>
            [XmlAttribute]
            public float AngleX;

            /// <summary>
            /// Camera Y-angle.
            /// </summary>
            [XmlAttribute]
            public float AngleY;

            /// <summary>
            /// Camera height.
            /// </summary>
            [XmlAttribute]
            public float Height;

            /// <summary>
            /// Camera size.
            /// </summary>
            [XmlAttribute]
            public float Size;

            /// <summary>
            /// Camera Field Of View.
            /// </summary>
            [XmlAttribute]
            public float FOV;
        }
    }
}