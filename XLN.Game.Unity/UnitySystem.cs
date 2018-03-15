using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace XLN.Game.Unity
{
    public static class UnitySystem
    {
        private static GameObject s_GameObject;
        private static readonly string s_SystemObjName = "XLNSystemObject";
        //private static SystemBehavior s_SystemBehavior;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
            UnityScheduler = TaskScheduler.FromCurrentSynchronizationContext();


            if (!s_GameObject)
            {
                s_GameObject = GameObject.Find(s_SystemObjName);
                if (!s_GameObject)
                {
                    s_GameObject = new GameObject(s_SystemObjName);
                    s_GameObject.hideFlags = HideFlags.HideInHierarchy;
                    UnityEngine.Object.DontDestroyOnLoad(s_GameObject);
                    SystemBehavior = s_GameObject.AddComponent<SystemBehavior>();
                    Debug.Log("System Object Added");

                }
            }

        }

        public static SystemBehavior SystemBehavior
        {
            get;
            private set;
        }

        public static void RunOnUnityScheduler(Action action)
        {
            if (SynchronizationContext.Current == UnitySystem.UnitySynchronizationContext)
            {
                action();
            }
            else
            {
                UnitySystem.UnitySynchronizationContext.Post(_ => action(), null);
            }
        }

 
        public static Task<R> RunTaskOnUnityScheduler<R>(Func<R> func)
        {
            if (SynchronizationContext.Current == UnitySystem.UnitySynchronizationContext)
            {
                return Task.FromResult(func());
            }
            else
            {
                var t = Task.Factory.StartNew(() => { return func(); }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, UnityScheduler);
                return t;
            }
        }


        public static int UnityThreadId
        {
            get; private set;
        }

        public static SynchronizationContext UnitySynchronizationContext
        {
            get; private set;
        }


        public static TaskScheduler UnityScheduler
        {
            get; private set;
        }

       

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoadRuntimeMethod()
        {
            
        }

    }
}
