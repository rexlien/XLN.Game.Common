/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
 * Contains some contributions under the Thrift Software License.
 * Please see doc/old-thrift-license.txt in the Thrift distribution for
 * details.
 */


using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;
using XLN.Game.Common;
using System.Threading.Tasks;
using XLN.Game.Common.Extension;
using static XLN.Game.Common.Extension.Extension;

namespace Thrift.Transport
{
    public class TAsyncSocket : TTransport
    {
        Socket socket = null;
        //static ThreadLocal<ManualResetEvent> readAsyncComplete = new ThreadLocal<ManualResetEvent>(()=>
        //{ return new ManualResetEvent(false); }

        public class SocketReceiveContext
        {
            public enum ReceiveType
            {
                HEADER,
                CONTENT
            };

            public ManualResetEvent m_ReadCompleteEvent = new ManualResetEvent(false);
            public UInt32 m_SeqID = 0;
            public ReceiveType m_ReceiveType;

            public MemoryStream m_InputStream;

        }

        public class PendingBufferEntry
        {
            //public UInt32 m_SeqID = 0;
            public MemoryStream m_HeaderBuffer;
            public MemoryStream m_ContentBuffer;
            //public ManualResetEvent m_ReceiveCompleteEvent = new ManualResetEvent(false);
            //public TaskCompletionSource<MemoryStream> m_Result = new TaskCompletionSource<MemoryStream>();
        }

        private static LimitedConcurrencyLevelTaskScheduler s_IOReadTaskSchduller = new LimitedConcurrencyLevelTaskScheduler(1);
        private static TaskFactory s_IoReadTaskFactory = new TaskFactory(s_IOReadTaskSchduller);

        private static LimitedConcurrencyLevelTaskScheduler s_IOWriteTaskSchduller = new LimitedConcurrencyLevelTaskScheduler(1);
        private static TaskFactory s_IoWriteTaskFactory = new TaskFactory(s_IOReadTaskSchduller);
                                                                                                 
        public event EventHandler<SocketAsyncEventArgs> connectHandler = null;

        // memory stream for write cache.
        private ThreadLocal<MemoryStream> outputStream = new ThreadLocal<MemoryStream>();
        private ConcurrentQueue<MemoryStream> m_StreamPool = new ConcurrentQueue<MemoryStream>();

        //private ConcurrentDictionary<UInt32, SocketReceiveContext> m_ReciveContentBuffer = new ConcurrentDictionary<UInt32, SocketReceiveContext>();
        private ConcurrentDictionary<UInt32, TaskCompletionSource<PendingBufferEntry>> m_PendingBuffers = new ConcurrentDictionary<UInt32, TaskCompletionSource<PendingBufferEntry>>();

        //private ConcurrentQueue<MemoryStream> m_PendingStream = new ConcurrentQueue<MemoryStream>();

        private string host = null;
        private int port = 0;
        private int timeout = 0;

        // constructor
        public TAsyncSocket(string host, int port)
            : this(host, port, 0)
        {
        }

        // constructor
        public TAsyncSocket(string host, int port, int timeout)
        {
            this.host = host;
            this.port = port;
            this.timeout = timeout;

            InitSocket();
        }

        private void InitSocket()
        {
            // Create a stream-based, TCP socket using the InterNetwork Address Family.
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
        }

        public int Timeout
        {
            set
            {
                timeout = value;
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

        public override bool IsOpen
        {
            get
            {
                if (socket == null)
                {
                    return false;
                }

                return socket.Connected;
            }
        }

        public ConcurrentDictionary<uint, TaskCompletionSource<PendingBufferEntry>> PendingBuffers { get => m_PendingBuffers; set => m_PendingBuffers = value; }

        public override void Open()
        {
            if (IsOpen)
            {
                throw new TTransportException(TTransportException.ExceptionType.AlreadyOpen, "Socket already connected");
            }

            if (String.IsNullOrEmpty(host))
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot open null host");
            }

            if (port <= 0)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot open without port");
            }

            if (socket == null)
            {
                InitSocket();
            }

            if (timeout == 0)     // no timeout -> infinite
            {
                timeout = 10000;  // set a default timeout for WP.
            }

            {
                // Create DnsEndPoint. The hostName and port are passed in to this method.
                DnsEndPoint hostEntry = new DnsEndPoint(this.host, this.port);

                // Create a SocketAsyncEventArgs object to be used in the connection request
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = hostEntry;

                // Inline event handler for the Completed event.
                // Note: This event handler was implemented inline in order to make this method self-contained.
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
                {
                    if (connectHandler != null)
                    {
                        connectHandler(this, e);
                    }
                });

                // Make an asynchronous Connect request over the socket
                socket.ConnectAsync(socketEventArg);
            }
        }

        public override int Read(byte[] buf, int off, int len)
        {
            throw new NotImplementedException();
        }

        public void ProcessHeaderBuffer()
        {
            
        }

        public void ProcesContenBuffer()
        {
            
        }

        public Task<MemoryStream> UnFrame(UInt32 seqID)
        {
           
            //SocketReceiveContext receiveContext = null;
            //socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
            EventHandler<SocketAsyncEventArgs> h = (o, ea) =>
            {
                SocketReceiveContext receiveContext = (SocketReceiveContext)ea.UserToken;
                LogService.Logger.Log(LogService.LogType.LT_DEBUG, "ReciveComplete SeqID: "+ receiveContext.m_SeqID.ToString());

                if (ea.SocketError == SocketError.Success)
                {
                    
                   
                }
                else
                {
                    
                }
                receiveContext.m_ReadCompleteEvent.Set();


            };
          
            // Sets the state of the event to nonsignaled, causing threads to block
            //inContext.m_ReadCompleteEvent.Reset();
            //Task
            SocketReceiveContext headerReceiveContext = new SocketReceiveContext();
            SocketReceiveContext contentReceiveContext = new SocketReceiveContext();
            TaskCompletionSource<MemoryStream> retSource = new TaskCompletionSource<MemoryStream>();
            //headerReceiveContext.m_ReceiveType = SocketReceiveContext.ReceiveType.HEADER;

            s_IoReadTaskFactory.StartNew(() =>
            {
                SocketAsyncEventArgs headerEventArg = new SocketAsyncEventArgs();

                MemoryStream headerStream = new MemoryStream(12);
                headerStream.SetLength(4);
                //headerReceiveContext.m_InputStream = headerStream;

                headerEventArg.SetBuffer(headerStream.GetBuffer(), 0, 4);
                headerEventArg.UserToken = headerReceiveContext;

                headerEventArg.Completed += h;

                lock(socket)
                {
                    socket.ReceiveAsync(headerEventArg);
                    if (!headerReceiveContext.m_ReadCompleteEvent.WaitOne(timeout))
                    {
                        retSource.SetException(new TTransportException(TTransportException.ExceptionType.TimedOut, "Socket recv timeout"));
                        return;
                    }
                    uint frameSize = 0;
                    using (BinaryReader reader = new BinaryReader(headerStream, System.Text.Encoding.Default, true))
                    {
                        frameSize = reader.ntoh32();
                        //if (frameSize > MAX_FRAME_SIZE)
                        {
                            //TODO 
                            //headerStream.SetLength(12);
                            //m_AsyncSocket.ReadWithContext(headerStream.GetBuffer(), 4, 8, receiveContext);
                        }
                        //else
                        {

                        }

                    }
                   

                    SocketAsyncEventArgs contentEventArg = new SocketAsyncEventArgs();
                    MemoryStream content = new MemoryStream((int)frameSize);
                    content.SetLength(frameSize);
                    contentReceiveContext.m_InputStream = content;

                    contentEventArg.UserToken = contentReceiveContext;
                    contentEventArg.SetBuffer(content.GetBuffer(), 0, (int)frameSize);
                    contentEventArg.Completed += h;

                    socket.ReceiveAsync(contentEventArg);


                    if (!contentReceiveContext.m_ReadCompleteEvent.WaitOne(timeout))
                    {
                        retSource.SetException(new TTransportException(TTransportException.ExceptionType.TimedOut, "Socket recv timeout"));
                    }
                    else
                    {
                        retSource.SetResult(content);
                    }


                }

                //LogService.Logger.Log(LogService.LogType.LT_DEBUG, "io read task leave");

            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            // Make an asynchronous Receive request over the socket
            //socket.ReceiveAsync(socketEventArg);

            // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
            // If no response comes back within this time then proceed
            //inContext.m_ReadCompleteEvent.WaitOne(this.timeout);

           

           
            //if (inContext.m_SeqID != receiveContext.m_SeqID)
            //{
            //    throw new TTransportException("sequence id not matched");
            //}

            return retSource.Task;
        }

        public override void Write(byte[] buf, int off, int len)
        {
            if(outputStream.Value == null)
            {
                MemoryStream stream;
                m_StreamPool.TryDequeue(out stream);
                if(stream != null)
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

        private void beginFlush_Completed(object sender, SocketAsyncEventArgs e)
        {
            FlushAsyncResult flushAsyncResult = e.UserToken as FlushAsyncResult;
            flushAsyncResult.UpdateStatusToComplete();
            flushAsyncResult.NotifyCallbackWhenAvailable();

            if (e.SocketError != SocketError.Success)
            {
                throw new TTransportException(TTransportException.ExceptionType.Unknown, e.SocketError.ToString());
            }
        }



        public IAsyncResult BeginFlush(AsyncCallback callback, object state, UInt32 seqID)
        {
            // Extract request and reset buffer

            LogService.Logger.Log(LogService.LogType.LT_DEBUG, "flush " + seqID.ToString() + " size: " + outputStream.Value.Length);

            byte[] data = outputStream.Value.ToArray();
            m_StreamPool.Enqueue(outputStream.Value);
            outputStream.Value = null;

            FlushAsyncResult flushAsyncResult = new FlushAsyncResult(callback, state);
            flushAsyncResult.SeqID = seqID;

            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.UserToken = flushAsyncResult;

            socketEventArg.Completed += beginFlush_Completed;
            socketEventArg.SetBuffer(data, 0, data.Length);

            s_IoWriteTaskFactory.StartNew(() =>
            {
                socket.SendAsync(socketEventArg);
            });
            return flushAsyncResult;
        }

        public override void EndFlush(IAsyncResult asyncResult)
        {

            var flushAsyncResult = (FlushAsyncResult)asyncResult;

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


        // Copy from impl from THttpClient.cs
        // Based on http://msmvps.com/blogs/luisabreu/archive/2009/06/15/multithreading-implementing-the-iasyncresult-interface.aspx
        public class FlushAsyncResult : IAsyncResult
        {
            private volatile Boolean _isCompleted;
            private ManualResetEvent _evt;
            private readonly AsyncCallback _cbMethod;
            private readonly Object _state;
            private UInt32 m_SeqID;

            public FlushAsyncResult(AsyncCallback cbMethod, Object state)
            {
                _cbMethod = cbMethod;
                _state = state;
            }

            internal byte[] Data { get; set; }
            internal Socket Connection { get; set; }
            internal TTransportException AsyncException { get; set; }

            public object AsyncState
            {
                get { return _state; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get { return GetEvtHandle(); }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return _isCompleted; }
            }

            public uint SeqID { get => m_SeqID; set => m_SeqID = value; }

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

            internal void UpdateStatusToComplete()
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

            internal void NotifyCallbackWhenAvailable()
            {
                if (_cbMethod != null)
                {
                    _cbMethod(this);
                }
            }
        }

        public override void Close()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
        }

        #region " IDisposable Support "
        private bool _IsDisposed;

        // IDisposable
        protected override void Dispose(bool disposing)
        {
            if (!_IsDisposed)
            {
                if (disposing)
                {
                    if (outputStream != null)
                    {
                        outputStream.Dispose();
                    }
                    outputStream = null;
                    if (socket != null)
                    {
                        ((IDisposable)socket).Dispose();
                    }
                }
            }
            _IsDisposed = true;
        }
        #endregion
    }
}


