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
