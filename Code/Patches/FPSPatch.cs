using System;
using System.Reflection;
using UnityEngine;
using ColossalFramework;
using HarmonyLib;


namespace ACME
{
    /// <summary>
    /// Harmony patch to replace CameraControler.LateUpdate for FPS mode.
    /// </summary>
    //[HarmonyPatch]
    public static class FPSPatch
    {
        public static Vector3 cameraPosition;
        public static Vector2 cameraRotation;


        public static MethodBase TargetMethod()
        {

            MethodBase targetMethod = typeof(CameraController).GetMethod("FpsBoosterLateUpdate", BindingFlags.Public | BindingFlags.Instance);

            //MethodInfo targetMethod = typeof(CameraController).GetMethod("LateUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
            return targetMethod;
        }

        public static bool Prefix(CameraController __instance)
        {
            Vector3 direction = Vector3.zero;

            cameraRotation = __instance.m_currentAngle;

            if (Input.GetKey(KeyCode.Q))
            {
                cameraRotation.x += 20f * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.E))
            {
                cameraRotation.x -= 20f * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.R))
            {
                cameraRotation.y -= 20f * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.F))
            {
                cameraRotation.y += 20f * Time.deltaTime;
            }

            __instance.m_currentAngle = cameraRotation;
            __instance.m_targetAngle = cameraRotation;

            Quaternion quaternion = Quaternion.AngleAxis(cameraRotation.x, Vector3.up) * Quaternion.AngleAxis(cameraRotation.y, Vector3.right);


            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }

            cameraPosition += quaternion * direction * 50f * Time.deltaTime;



            //vector = CameraController.ClampCameraPosition(vector);
            __instance.transform.rotation = quaternion;
            __instance.transform.position = cameraPosition;
            __instance.m_currentPosition = cameraPosition;
            __instance.m_targetPosition = cameraPosition;
            return false;
        }
    }
}