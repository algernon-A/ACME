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
            if (ModUtils.miGetTotalBounds != null)
            {
                // Get Move It bounds.
                if (ModUtils.miGetTotalBounds.Invoke(null, new object[] {false, false}) is Bounds miBounds)
                {
                    // Don't do anything if bounds haven't been set.
                    if (miBounds.size != Vector3.zero)
                    {
                        // Move camera to point at centre of Move It selection. 
                        CameraController controller = Camera.main.GetComponent<CameraController>();
                        controller.m_targetPosition = miBounds.center;
                    }
                }

            }
            
            /*
            Logging.KeyMessage("Attempting to find MoveIt");
            Assembly moveItAsm = ModUtils.GetEnabledAssembly("MoveIt");

            if (moveItAsm != null)
            {
                Type moveItSelectAction = moveItAsm.GetType("MoveIt.SelectAction");

                if (moveItSelectAction != null)
                {
                    Logging.KeyMessage("found MoveIt.SelectAction");
                }

                Type moveItActionQueue = moveItAsm.GetType("MoveIt.ActionQueue");
                if (moveItActionQueue != null)
                {
                    Logging.KeyMessage("found MoveIt.ActionQueue");

                    FieldInfo actionQueueInstance = moveItActionQueue.GetField("instance", BindingFlags.Static | BindingFlags.Public);
                    if (actionQueueInstance != null)
                    {
                        Logging.Message("found actionQueueInstance");

                        if (actionQueueInstance.GetValue(null) is ActionQueue actionQueue)
                        {
                            PropertyInfo currentAction = moveItActionQueue.GetProperty("current");
                            if (currentAction != null)
                            {

                                Logging.Message("found currentAction");
                                object thisAction = currentAction.GetValue(actionQueue, null);

                                if (thisAction!= null)
                                {
                                    Logging.Message("current action is ", thisAction.ToString());

                                    if (thisAction is MoveIt.Action fred)
                                    {
                                        Logging.Message("Got MoveItAction");

                                        Bounds moveItBounds = MoveIt.Action.GetTotalBounds();

                                        CameraController controller = Camera.main.GetComponent<CameraController>();
                                        controller.m_targetPosition = moveItBounds.center;
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }*/
        }
    }
}
;