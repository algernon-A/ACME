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
            // Local reference.
            CameraController controller = CameraUtils.Controller;

            // Save current camera attributes.
            savedPositions[positionIndex] = new SavedPosition
            {
                isValid = true,
                position = controller.m_targetPosition,
                angle = controller.m_targetAngle,
                height = controller.m_targetHeight,
                size = controller.m_currentSize,
                fov = Camera.main.fieldOfView
            };
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
            writer.Write((int)0);

            // Serialise each position.
            for (int i = 0; i < NumSaves; ++i)
            {
                // Serialize key and simple fields.
                writer.Write(savedPositions[i].isValid);
                writer.Write(savedPositions[i].position.x);
                writer.Write(savedPositions[i].position.y);
                writer.Write(savedPositions[i].position.z);
                writer.Write(savedPositions[i].angle.x);
                writer.Write(savedPositions[i].angle.y);
                writer.Write(savedPositions[i].height);
                writer.Write(savedPositions[i].size);
                writer.Write(savedPositions[i].fov);
            }
        }


        /// <summary>
        /// Deserializes savegame data.
        /// </summary>
        /// <param name="stream">Data memory stream to deserialize from</param>
        internal static void Deserialize(BinaryReader reader)
        {
            Logging.Message("deserializing savegame data");

            // Read version.
            reader.ReadInt32();

            // Serialise each position.
            for (int i = 0; i < NumSaves; ++i)
            {
                // Serialize key and simple fields.
                savedPositions[i].isValid = reader.ReadBoolean();
                savedPositions[i].position.x = reader.ReadSingle();
                savedPositions[i].position.y = reader.ReadSingle();
                savedPositions[i].position.z = reader.ReadSingle();
                savedPositions[i].angle.x = reader.ReadSingle();
                savedPositions[i].angle.y = reader.ReadSingle();
                savedPositions[i].height = reader.ReadSingle();
                savedPositions[i].size = reader.ReadSingle();
                savedPositions[i].fov = reader.ReadSingle();
            }
        }
    }
}