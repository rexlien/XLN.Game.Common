using System;
using UnityEngine;

namespace XLN.Game.Unity
{
    public class System
    {
        private static GameObject s_GameObject;
        private static readonly string s_SystemObjName = "XLNSystemObject";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            
            if (!s_GameObject)
            {
                s_GameObject = GameObject.Find(s_SystemObjName);
                if (!s_GameObject)
                {
                    s_GameObject = new GameObject(s_SystemObjName);
                    s_GameObject.hideFlags = HideFlags.HideAndDontSave;
                    UnityEngine.Object.DontDestroyOnLoad(s_GameObject);
                    s_GameObject.AddComponent<SystemBehavior>();
                    Debug.Log("System Object Added");

                }
            }

        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoadRuntimeMethod()
        {
            
        }

    }
}
