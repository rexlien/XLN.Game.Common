using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Thrift.Transport;

namespace XLN.Game.Common.Thrift
{
    public abstract class TAsyncSocket : TTransport
    {
        
        public TAsyncSocket(string host, int port)
            :this(host, port, 0)
        {
            
        }

        public TAsyncSocket(string host, int port, int timeout)
        {
            this.host = host;
            this.port = port;
            this.Timeout = timeout;
        }

        public class PendingBufferEntry
        {
            //public UInt32 m_SeqID = 0;
            public MemoryStream m_HeaderBuffer;
            public MemoryStream m_ContentBuffer;
            //public ManualResetEvent m_ReceiveCompleteEvent = new ManualResetEvent(false);
            //public TaskCompletionSource<MemoryStream> m_Result = new TaskCompletionSource<MemoryStream>();
        }
        protected ConcurrentDictionary<UInt32, TaskCompletionSource<PendingBufferEntry>> m_PendingBuffers = new ConcurrentDictionary<UInt32, TaskCompletionSource<PendingBufferEntry>>();
        public ConcurrentDictionary<uint, TaskCompletionSource<PendingBufferEntry>> PendingBuffers { get => m_PendingBuffers; set => m_PendingBuffers = value; }
        protected ThreadLocal<MemoryStream> outputStream = new ThreadLocal<MemoryStream>();
        protected ConcurrentQueue<MemoryStream> m_StreamPool = new ConcurrentQueue<MemoryStream>();


        protected int timeout = 0;
        protected string host = null;
        protected int port = 0;
        public int Timeout
        {
            set
            {
                timeout = value;
                if(timeout == 0)
                {
                    timeout = Int32.MaxValue;
                }
            }

            get
            {
                return timeout;
            }
        }

        public string Host
        {
            get
            {
                return host;
            }
        }

        public int Port
        {
            get
            {
                return port;
            }
        }

        //public override bool IsOpen => throw new NotImplementedException();

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Open()
        {
            OpenAsyncTask().Wait();
        }

        public override int Read(byte[] buf, int off, int len)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buf, int off, int len)
        {
            if (outputStream.Value == null)
            {
                MemoryStream stream;
                m_StreamPool.TryDequeue(out stream);
                if (stream != null)
                {
                    stream.SetLength(0);
                    stream.Seek(0, SeekOrigin.Begin);
                    outputStream.Value = stream;
                }
                else
                {
                    outputStream.Value = new MemoryStream(1024);
                }
            }
            outputStream.Value.Write(buf, off, len);
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        public abstract Task<bool> OpenAsyncTask();
        public abstract Task<MemoryStream> UnFrame(UInt32 seqID);

        public virtual async Task OpenAsync()
        {
            TaskCompletionSource<bool> retSource = new TaskCompletionSource<bool>();

            Task<bool> openTask = OpenAsyncTask();
            Task timeoutTask = Task.Delay(timeout);
            Task complete = null;
            try
            {
                complete = await Task.WhenAny(openTask, timeoutTask);
                await complete;
            }
            catch (TTransportException ex)
            {
                throw ex;
            }
            if (complete == timeoutTask)
            {
                throw new TTransportException("Connection Timeout");
            }
        }

        public abstract IAsyncResult BeginFlush(AsyncCallback callback, object state, UInt32 seqID);
        public override void EndFlush(IAsyncResult asyncResult)
        {

            var flushAsyncResult = (SocketFlushAsyncResult)asyncResult;

            if (!flushAsyncResult.IsCompleted)
            {
                var waitHandle = flushAsyncResult.AsyncWaitHandle;
                waitHandle.WaitOne();
                waitHandle.Close();
            }

            if (flushAsyncResult.AsyncException != null)
            {
                throw flushAsyncResult.AsyncException;
            }

        }


        public class SocketFlushAsyncResult : IAsyncResult
        {
            

            protected UInt32 m_SeqID;
            public uint SeqID { get => m_SeqID; set => m_SeqID = value; }
            private volatile Boolean _isCompleted;
            private ManualResetEvent _evt;
            private readonly AsyncCallback _cbMethod;
            private readonly Object _state;
            //private UInt32 m_SeqID;

            public SocketFlushAsyncResult(AsyncCallback cbMethod, Object state)
            {
                _cbMethod = cbMethod;
                _state = state;
            }

            internal byte[] Data { get; set; }
            //internal Socket Connection { get; set; }
            internal TTransportException AsyncException { get; set; }

            public  object AsyncState
            {
                get { return _state; }
            }

            public  WaitHandle AsyncWaitHandle
            {
                get { return GetEvtHandle(); }
            }

            public  bool CompletedSynchronously
            {
                get { return false; }
            }

            public  bool IsCompleted
            {
                get { return _isCompleted; }
            }


            private readonly Object _locker = new Object();

            private ManualResetEvent GetEvtHandle()
            {
                lock (_locker)
                {
                    if (_evt == null)
                    {
                        _evt = new ManualResetEvent(false);
                    }
                    if (_isCompleted)
                    {
                        _evt.Set();
                    }
                }
                return _evt;
            }

            public void UpdateStatusToComplete()
            {
                _isCompleted = true; //1. set _iscompleted to true
                lock (_locker)
                {
                    if (_evt != null)
                    {
                        _evt.Set(); //2. set the event, when it exists
                    }
                }
            }

            public void NotifyCallbackWhenAvailable()
            {
                if (_cbMethod != null)
                {
                    _cbMethod(this);
                }
            }
        }




    }
}
