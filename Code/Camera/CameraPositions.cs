using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace ACME
{
	/// <summary>
	/// Structure to store saved camera positions.
	/// </summary>
	public struct SavedPosition
	{
		public bool isValid;
		public Vector3 position;
		public Vector2 angle;
		public float height;
		public float size;
		public float fov;
	}


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
            if (savedPositions[positionIndex].isValid)
            {
                // Local reference.
                CameraController controller = CameraUtils.Controller;

                // Restore saved attributes.
                controller.m_targetPosition = savedPositions[positionIndex].position;
                controller.m_targetAngle = savedPositions[positionIndex].angle;
                controller.m_targetHeight = savedPositions[positionIndex].height;
                controller.m_targetSize = savedPositions[positionIndex].size;
                Camera.main.fieldOfView = savedPositions[positionIndex].fov;
            }
        }
        /// <summary>
        /// Serializes savegame data.
        /// </summary>
        /// <param name="stream">Binary writer instance to serialize to</param>
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
        /// <param name="stream">Data memory stream to deserialize from</param>
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
        /// <returns>New SavedPosition struct reflecting current camera position</returns>
        private static SavedPosition CurrentPosition()
        {
            // Local reference.
            CameraController controller = CameraUtils.Controller;

            // Save current camera attributes.
            return new SavedPosition
            {
                isValid = true,
                position = controller.m_targetPosition,
                angle = controller.m_targetAngle,
                height = controller.m_targetHeight,
                size = controller.m_currentSize,
                fov = CameraUtils.MainCamera.fieldOfView
            };
        }


        /// <summary>
        /// Serializes a camera position to the given BinaryWriter.
        /// </summary>
        /// <param name="position">Position to serialize</param>
        /// <param name="writer">Target BinaryWriter</param>
        private static void WritePosition(SavedPosition position, BinaryWriter writer)
        {
            writer.Write(position.isValid);
            writer.Write(position.position.x);
            writer.Write(position.position.y);
            writer.Write(position.position.z);
            writer.Write(position.angle.x);
            writer.Write(position.angle.y);
            writer.Write(position.height);
            writer.Write(position.size);
            writer.Write(position.fov);
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
                isValid = reader.ReadBoolean(),
                position = new Vector3
                {
                    x = reader.ReadSingle(),
                    y = reader.ReadSingle(),
                    z = reader.ReadSingle()
                },
                angle = new Vector2
                {
                    x = reader.ReadSingle(),
                    y = reader.ReadSingle()
                },
                height = reader.ReadSingle(),
                size = reader.ReadSingle(),
                fov = reader.ReadSingle()
            };
        }
    }
}