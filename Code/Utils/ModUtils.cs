using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using ICities;
using ColossalFramework.Plugins;


namespace ACME
{
    /// <summary>
    /// Class that manages interactions with other mods, including compatibility and functionality checks.
    /// </summary>
    internal static class ModUtils
    {
        // Move It ActionQueue current action getter.
        internal static MethodInfo miGetTotalBounds;


        /// <summary>
        /// Returns the filepath of the current mod assembly.
        /// </summary>
        /// <returns>Mod assembly filepath</returns>
        internal static string GetAssemblyPath()
        {
            // Get list of currently active plugins.
            IEnumerable<PluginManager.PluginInfo> plugins = PluginManager.instance.GetPluginsInfo();

            // Iterate through list.
            foreach (PluginManager.PluginInfo plugin in plugins)
            {
                try
                {
                    // Get all (if any) mod instances from this plugin.
                    IUserMod[] mods = plugin.GetInstances<IUserMod>();

                    // Check to see if the primary instance is this mod.
                    if (mods.FirstOrDefault() is ACMEMod)
                    {
                        // Found it! Return path.
                        return plugin.modPath;
                    }
                }
                catch
                {
                    // Don't care.
                }
            }

            // If we got here, then we didn't find the assembly.
            Logging.Error("assembly path not found");
            throw new FileNotFoundException(ACMEMod.ModName + ": assembly path not found!");
        }


        /// <summary>
        /// Checks to see if another mod is installed and enabled, based on a provided assembly name, and if so, returns the assembly reference.
        /// Case-sensitive!  PloppableRICO is not the same as ploppablerico!
        /// </summary>
        /// <param name="assemblyName">Name of the mod assembly</param>
        /// <returns>Assembly reference if target is found and enabled, null otherwise</returns>
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
        /// Attempts to find the GetTotalBounds method of Move It's current action item.
        /// </summary>
        internal static void MoveItReflection()
        {
            Logging.KeyMessage("Attempting to find Move It");

            // Get assembly.
            Assembly miAssembly = ModUtils.GetEnabledAssembly("MoveIt");

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
