using System.Reflection;
using UnityEngine;


namespace ACME
{
	/// <summary>
	/// Utility class for changing camera settings.
	/// </summary>
	public static class CameraUtils
	{
		// Minimum and maximum near clip distance.
		internal const float MinNearClip = 0.1f;
		internal const float MaxNearClip = 10f;

		// Minimum and maximum distance.
		internal const float MinDistance = 2;
		internal const float MaxDistance = 44000f;

		// Current near clip plane distance (game default is 5).
		private static float nearClipPlane = 1f;

		// CameraManager m_originalNearPlane.
		private readonly static FieldInfo originalNearPlane = typeof(CameraController).GetField("m_originalNearPlane", BindingFlags.NonPublic | BindingFlags.Instance);

		// CameraController reference.
		private static CameraController cameraController;

		// Main camera reference.
		private static Camera mainCamera;

		// Initial camera position.
		internal static SavedPosition initialPosition = default;


		/// <summary>
		/// CameraController reference.
		/// </summary>
		internal static CameraController Controller
        {
			get
            {
				// Get camera controller if we haven't already, and main camera has been instantiated.
				if (cameraController == null && Camera.main != null)
                {
					cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
				}
				return cameraController;
			}
        }


		/// <summary>
		/// Main camera reference.
		/// </summary>
		internal static Camera MainCamera
        {
			get
            {
				// Get main camera if we haven't already.
				if (mainCamera == null)
				{
					mainCamera = Controller.GetComponent<Camera>();
				}
				return mainCamera;
			}
        }


		/// <summary>
		/// Sets the camera controllers base near clip plane base distance, in metres.
		/// Applied to the game's camera controller (not directly to the main camera), as that clobbers the camera's nearClipPlane every LateUpdate.
		/// </summary>
		public static float NearClipPlane
		{
			get => nearClipPlane;

			set
			{
				// Clamp value to min and max permitted.
				nearClipPlane = Mathf.Clamp(MinNearClip, value, MaxNearClip);

				// Set field with updated value (only if game has loaded, e.g. Controller isn't null).
				CameraController controller = Controller;
				if (controller != null)
				{
					originalNearPlane.SetValue(controller, nearClipPlane);
				}
			}
		}


		/// <summary>
		/// Applies the current cammera settings to the game camera.
		/// Should only be called once level has loaded.
		/// </summary>
		public static void ApplySettings()
		{
			// Local reference.
			CameraController controller = Controller;

			// Remove border limts.
			controller.m_unlimitedCamera = true;

			// Set camera minimum and maximum distance from target.
			controller.m_minDistance = MinDistance;
			controller.m_maxDistance = MaxDistance;

			// Apply near clip plane.
			NearClipPlane = nearClipPlane;

			// Set initial camera position, if we have one.
			if (initialPosition.isValid)
            {
				controller.m_targetPosition = initialPosition.position;
				controller.m_targetAngle = initialPosition.angle;
				controller.m_targetSize = initialPosition.size;
				controller.m_targetHeight = initialPosition.height;
				controller.m_targetSize = initialPosition.size;

				if (MainCamera != null)
				{
					MainCamera.fieldOfView = initialPosition.fov;
				}
				else
                {
					Logging.Error("unable to locate main camera");
                }
            }
		}
	}
}