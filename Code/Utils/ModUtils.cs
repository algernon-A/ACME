// <copyright file="ModUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System;
    using System.Reflection;
    using AlgernonCommons;
    using ColossalFramework.Plugins;

    /// <summary>
    /// Class that manages interactions with other mods.
    /// </summary>
    internal static class ModUtils
    {
        // Move It methods.
        internal static MethodInfo miGetTotalBounds, miGetAngle;

        /// <summary>
        /// Checks to see if another mod is installed and enabled, based on a provided assembly name, and if so, returns the assembly reference.
        /// Case-sensitive!  PloppableRICO is not the same as ploppablerico!
        /// </summary>
        /// <param name="assemblyName">Name of the mod assembly.</param>
        /// <returns>Assembly reference if target is found and enabled, null otherwise.</returns>
        internal static Assembly GetEnabledAssembly(string assemblyName)
        {
            // Iterate through the full list of plugins.
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                // Only looking at enabled plugins.
                if (plugin.isEnabled)
                {
                    foreach (Assembly assembly in plugin.GetAssemblies())
                    {
                        if (assembly.GetName().Name.Equals(assemblyName))
                        {
                            Logging.Message("found enabled mod assembly ", assemblyName, ", version ", assembly.GetName().Version);
                            return assembly;
                        }
                    }
                }
            }

            // If we've made it here, then we haven't found a matching assembly.
            Logging.Message("didn't find enabled assembly ", assemblyName);
            return null;
        }

        /// <summary>
        /// Attempts to find the GetTotalBounds and GetAngle methods of Move It's current action item.
        /// </summary>
        internal static void MoveItReflection()
        {
            Logging.KeyMessage("Attempting to find Move It");

            // Get assembly.
            Assembly miAssembly = GetEnabledAssembly("MoveIt");

            if (miAssembly == null)
            {
                Logging.Message("Move It not found");
                return;
            }

            // Action class.
            Type miAction = miAssembly.GetType("MoveIt.Action");
            if (miAction == null)
            {
                Logging.Error("MoveIt.Action not reflected");
                return;
            }

            // GetTotalBounds method.
            miGetTotalBounds = miAction.GetMethod("GetTotalBounds", BindingFlags.Static | BindingFlags.Public);
            if (miGetTotalBounds == null)
            {
                Logging.Error("MoveIt.Action.GetTotalBounds not reflected");
                return;
            }

            // GetAngle method.
            miGetAngle = miAction.GetMethod("GetAngle", BindingFlags.Static | BindingFlags.Public);
            if (miGetAngle == null)
            {
                Logging.Error("MoveIt.Action.GetAngle not reflected");
                return;
            }

            Logging.Message("Move It reflection complete");


            /*
            // SelectAction class.
            Type miSelectAction = miAssembly.GetType("MoveIt.SelectAction");
            if (miSelectAction == null)
            {
                Logging.Error("MoveIt.SelectAction not reflected");
                return;
            }

            // ActionQueue class.
            Type miActionQueueClass = miAssembly.GetType("MoveIt.ActionQueue");
            if (miActionQueueClass == null)
            {
                Logging.Error("MoveIt.ActionQueue not reflected");
                return;
            }

            // Current action getter.
            miCurrentAction = miActionQueue.GetProperty("current");
            if (miCurrentAction == null)
            {
                Logging.Error("MoveIt.ActionQueue.current not reflected");
                return;
            }

            // Action bounds method.
            MethodInfo miTotalBounds = miSelectAction.GetMethod("GetTOtal")

            // ActionQueue instance getter.
            FieldInfo miActionQueueInstanceField = miActionQueue.GetField("instance", BindingFlags.Static | BindingFlags.Public);
            if (miActionQueueInstanceField == null)
            {
                Logging.Error("MoveIt.ActionQueue.instance not reflected");
                return;
            }

            // Set ActionQueue instance reference.
            miActionQueue = miActionQueueInstanceField.GetValue(null);

            // Set Curren



            if (currentAction != null)
                            {

                                Logging.Message("found currentAction");
                                object thisAction = currentAction.GetValue(actionQueue, null);

                                if (thisAction != null)
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
