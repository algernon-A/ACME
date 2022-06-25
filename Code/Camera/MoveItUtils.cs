using UnityEngine;


namespace ACME
{
    /// <summary>
    /// Class to handle integration with Move It mod.
    /// </summary>
    internal static class MoveItUtils
    {
        /// <summary>
        /// Moves the current camera position to the last stored Move It position.
        /// </summary>
        internal static void SetPosition()
        {
            // Don't do anything if Move It wasn't reflected.
            if (ModUtils.miGetTotalBounds == null || ModUtils.miGetAngle == null)
            {
                return;
            }

            // Get Move It bounds.
            if (ModUtils.miGetTotalBounds.Invoke(null, new object[] { false, false }) is Bounds miBounds)
            {
                // Don't do anything if bounds haven't been set.
                if (miBounds.size != Vector3.zero)
                {
                    // Move camera to point at centre of Move It selection. 
                    CameraController controller = CameraUtils.Controller;
                    if (controller != null)
                    {
                        controller.m_targetPosition = miBounds.center;

                        // Rotate camera to point directly at front of selection.
                        if (ModUtils.miGetAngle.Invoke(null, null) is float miAngle)
                        {
                            controller.m_targetAngle.x = ((miAngle * -Mathf.Rad2Deg) + 180) % 360;
                        }
                    }
                    else
                    {
                        Logging.Error("unable to get CameraController for MoveItUtils.SetPosition");
                    }
                }
            }
        }
    }
}