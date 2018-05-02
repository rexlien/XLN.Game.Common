using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using XLN.Game.Common.Utils;

namespace XLN.Game.Common.Extension
{
    public static class Extension
    {


         public static TValue GetAttributeValue<TAttribute, TValue>( this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }


        public static UInt16 ntoh16(this BinaryReader reader)
        {
            return (UInt16)IPAddress.NetworkToHostOrder((short)reader.ReadUInt16());
        }

        public static Int16 ntohS16(this BinaryReader reader)
        {
            return IPAddress.NetworkToHostOrder(reader.ReadInt16());
        }

        public static UInt32 ntoh32(this BinaryReader reader)
        {
            return (UInt32)IPAddress.NetworkToHostOrder((int)reader.ReadUInt32());
        }

        public static Int32 ntohS32(this BinaryReader reader)
        {

            return IPAddress.NetworkToHostOrder(reader.ReadInt32());
        }

        public static Int32 ntohVarint32(this BinaryReader reader)
        {

            return IPAddress.NetworkToHostOrder((int)Varint.ReadVarint32(reader));
        }

        public static Int64 ntohVarint64(this BinaryReader reader)
        {

            return IPAddress.NetworkToHostOrder((long)Varint.ReadVarint64(reader));
        }


            public static SocketAwaitable ReceiveAsync(this Socket socket,
                SocketAwaitable awaitable)
            {
                awaitable.Reset();
                if (!socket.ReceiveAsync(awaitable.m_eventArgs))
                    awaitable.m_wasCompleted = true;
                return awaitable;
            }

            public static SocketAwaitable SendAsync(this Socket socket,
                SocketAwaitable awaitable)
            {
                awaitable.Reset();
                if (!socket.SendAsync(awaitable.m_eventArgs))
                    awaitable.m_wasCompleted = true;
                return awaitable;
            }





        public sealed class SocketAwaitable : INotifyCompletion
        {
            private readonly static Action SENTINEL = () => { };

            internal bool m_wasCompleted;
            internal Action m_continuation;
            internal SocketAsyncEventArgs m_eventArgs;

            public SocketAwaitable(SocketAsyncEventArgs eventArgs)
            {
                //if (eventArgs == null) throw new ArgumentNullException(“eventArgs”);
                m_eventArgs = eventArgs;
                eventArgs.Completed += delegate
                {
                    var prev = m_continuation ?? Interlocked.CompareExchange(
                        ref m_continuation, SENTINEL, null);
                    if (prev != null) prev();
                };
            }

            internal void Reset()
            {
                m_wasCompleted = false;
                m_continuation = null;
            }

            public SocketAwaitable GetAwaiter() { return this; }

            public bool IsCompleted { get { return m_wasCompleted; } }

            public void OnCompleted(Action continuation)
            {
                if (m_continuation == SENTINEL ||
                    Interlocked.CompareExchange(
                        ref m_continuation, continuation, null) == SENTINEL)
                {
                    Task.Run(continuation);
                }
            }

            public void GetResult()
            {
                if (m_eventArgs.SocketError != SocketError.Success)
                    throw new SocketException((int)m_eventArgs.SocketError);
            }
        }
    }
}
