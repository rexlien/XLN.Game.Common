using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XLN.Game.Common;

namespace XLN.Game.Unity
{
    public class WebGLSocket
    {
        public int Id { get; private set; }

        public string Error
        {
            get
            {
                const int bufferSize = 1024;
                byte[] buffer = new byte[bufferSize];
                int result = SocketError(buffer, bufferSize, Id);
                if (result == 0)
                    return null;
                return Encoding.UTF8.GetString(buffer);
            }
        }

        public int State { get { return SocketState(Id); } }

        public bool Connected { get => m_Connected; }
       
        [DllImport("__Internal")]
        private static extern void SocketCreate(string url, int id, Action<int> action);

        [DllImport("__Internal")]
        private static extern void SocketClose(int id);

        [DllImport("__Internal")]
        private static extern void SocketSend(byte[] data, int length, int id);

        [DllImport("__Internal")]
        private static extern int SocketState(int id);

        //[DllImport("__Internal")]
        //private static extern void SocketOnOpen(Action<int> action, int id);

        //[DllImport("__Internal")]
        //private static extern void SocketOnMessage(Action<int> action, int id);

        [DllImport("__Internal")]
        private static extern void SocketOnError(Action<int> action, int id);

        [DllImport("__Internal")]
        private static extern void SocketOnClose(Action<int, int> action, int id);

        [DllImport("__Internal")]
        private static extern void SocketReceive(byte[] ptr, int length, int id);

        [DllImport("__Internal")]
        private static extern int SocketReceiveLength(int id);

        [DllImport("__Internal")]
        private static extern int SocketError(byte[] ptr, int length, int id);

        static int s_UID = 0;
        static ConcurrentDictionary<int, Action<int>> s_PendingOpenActions = new ConcurrentDictionary<int, Action<int>>();


        private bool m_Connected = false;
        public WebGLSocket()
        {
            Id = s_UID++;

        }

        public void Connect(string url, Action<int> onConnect)
        {
            s_PendingOpenActions.TryAdd(Id, onConnect);
            SocketCreate(url, Id, OpenCallback);
        }

        public async Task<bool> ConnectAsync(string url)
        {
            TaskCompletionSource<bool> retSource = new TaskCompletionSource<bool>();
            Connect(url, (int id) => { m_Connected = true; retSource.SetResult(true); });
           
            return await retSource.Task;
        }


        public void SendAsync(byte[] packet)
        {
            SocketSend(packet, packet.Length, Id);
        }

        public void CloseAsync()
        {
            SocketClose(Id);
        }
      
        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OpenCallback(int id)
        {
            Action<int> outCB = null;
            s_PendingOpenActions.TryRemove(id, out outCB);
            if(outCB != null)
            {
                outCB(id);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnError(int id)
        {
            Action<int> outCB = null;
            s_PendingOpenActions.TryRemove(id, out outCB);
            if (outCB != null)
            {
                outCB(id);
            }
        }
        /*
        public void SetOnMessage(Action<int> action)
        {
            SocketOnMessage(action, Id);
        }
*/
        public void SetOnError(Action<int> action)
        {
            SocketOnError(action, Id);
        }

        public void SetOnClose(Action<int, int> action)
        {
            SocketOnClose(action, Id);
        }

        public byte[] Receive()
        {
            int length = SocketReceiveLength(Id);
            if (length == 0)
                return null;
            byte[] buffer = new byte[length];
            SocketReceive(buffer, length, Id);
            return buffer;
        }
    }

}
