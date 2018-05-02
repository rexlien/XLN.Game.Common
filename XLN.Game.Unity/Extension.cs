using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using XLN.Game.Common;

namespace XLN.Game.Unity.Extension
{
    public static class Extension
    {

        public static UnityEngine.Vector3 ToUnityVec3(this System.Numerics.Vector3 vec3)
        {
            return new UnityEngine.Vector3(vec3.X, vec3.Y, vec3.Z);
        }

        public static void ToUnityVec3(this System.Numerics.Vector3 vec3, UnityEngine.Vector3 unityVec)
        {
            unityVec.x = vec3.X;
            unityVec.y = vec3.Y;
            unityVec.z = vec3.Z;
             //new UnityEngine.Vector3(vec3.X, vec3.Y, vec3.Z);
        }

        public static UnityEngine.Vector2 ToUnityVec2(this System.Numerics.Vector2 vec2)
        {
            return new UnityEngine.Vector2(vec2.X, vec2.Y);
        }

        public static System.Numerics.Vector2 ToSystemVec2(this UnityEngine.Vector2 vec2)
        {
            return new System.Numerics.Vector2(vec2.x, vec2.y);
        }

        public static System.Numerics.Vector3 ToSystemVec3(this UnityEngine.Vector3 vec3)
        {
            return new System.Numerics.Vector3(vec3.x, vec3.y, vec3.z);
        }

        public static System.Numerics.Quaternion ToSystemQuatenion(this UnityEngine.Quaternion quaternion)
        {
            return new System.Numerics.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public static UnityEngine.Quaternion ToUnityQuatenion(this System.Numerics.Quaternion quaternion)
        {
            return new UnityEngine.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }


        public static string ResolveRealPath(this ResourcePath path)
        {
            if(path.Type == ResourcePath.PathType.Resource)
            {
                 return System.IO.Path.GetFileNameWithoutExtension(path.Path); 
            }
            return path.Path;
        }

        public static SimpleCoroutineAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest instruction)
        {
            SystemBehavior behavior = UnitySystem.SystemBehavior;

            var awaiter = new SimpleCoroutineAwaiter<AssetBundle>();
            UnitySystem.RunOnUnityScheduler(() => UnitySystem.SystemBehavior.StartCoroutine(
                InstructionWrappers.AssetBundleCreateRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<UnityEngine.Object> GetAwaiter(this AssetBundleRequest instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<UnityEngine.Object>();
            UnitySystem.RunOnUnityScheduler(() => UnitySystem.SystemBehavior.StartCoroutine(
                InstructionWrappers.AssetBundleRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<WWW> GetAwaiter(this WWW instruction)
        {
            return GetAwaiterReturnSelf(instruction);
        }

        public static SimpleCoroutineAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation instruction)
        {
            return GetAwaiterReturnSelf(instruction);
        }


        static SimpleCoroutineAwaiter<T> GetAwaiterReturnSelf<T>(T instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<T>();
            UnitySystem.RunOnUnityScheduler(() => UnitySystem.SystemBehavior.StartCoroutine(
                InstructionWrappers.ReturnSelf(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<UnityEngine.Object>();
            UnitySystem.RunOnUnityScheduler(() => UnitySystem.SystemBehavior.StartCoroutine(
                InstructionWrappers.ResourceRequest(awaiter, instruction)));
            return awaiter;
        }





        static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception("Assert hit!");
            }
        }

        public class SimpleCoroutineAwaiter<T> : INotifyCompletion
        {
            bool _isDone;
            Exception _exception;
            Action _continuation;
            T _result;

            public bool IsCompleted
            {
                get { return _isDone; }
            }

            public T GetResult()
            {
                Assert(_isDone);

                if (_exception != null)
                {
                    throw _exception;
                }

                return _result;
            }

            public void Complete(T result, Exception e)
            {
                Assert(!_isDone);

                _isDone = true;
                _exception = e;
                _result = result;

                // Always trigger the continuation on the unity thread when awaiting on unity yield
                // instructions
                if (_continuation != null)
                {
                    UnitySystem.RunOnUnityScheduler(_continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                Assert(_continuation == null);
                Assert(!_isDone);

                _continuation = continuation;
            }
        }

        public class SimpleCoroutineAwaiter : INotifyCompletion
        {
            bool _isDone;
            Exception _exception;
            Action _continuation;

            public bool IsCompleted
            {
                get { return _isDone; }
            }

            public void GetResult()
            {
                Assert(_isDone);

                if (_exception != null)
                {
                    throw _exception;
                }
            }

            public void Complete(Exception e)
            {
                Assert(!_isDone);

                _isDone = true;
                _exception = e;

                // Always trigger the continuation on the unity thread when awaiting on unity yield
                // instructions
                if (_continuation != null)
                {
                    UnitySystem.RunOnUnityScheduler(_continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                Assert(_continuation == null);
                Assert(!_isDone);

                _continuation = continuation;
            }
        }

        static class InstructionWrappers
        {
            public static IEnumerator ReturnVoid(
                SimpleCoroutineAwaiter awaiter, object instruction)
            {
                // For simple instructions we assume that they don't throw exceptions
                yield return instruction;
                awaiter.Complete(null);
            }

            public static IEnumerator AssetBundleCreateRequest(
                SimpleCoroutineAwaiter<AssetBundle> awaiter, AssetBundleCreateRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.assetBundle, null);
            }

            public static IEnumerator ReturnSelf<T>(
                SimpleCoroutineAwaiter<T> awaiter, T instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction, null);
            }

            public static IEnumerator AssetBundleRequest(
                SimpleCoroutineAwaiter<UnityEngine.Object> awaiter, AssetBundleRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }

            public static IEnumerator ResourceRequest(
                SimpleCoroutineAwaiter<UnityEngine.Object> awaiter, ResourceRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }
        }
    }

}
