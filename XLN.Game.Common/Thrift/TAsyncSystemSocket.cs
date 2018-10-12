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
using Force.Crc32;
using Thrift.Transport;

namespace XLN.Game.Common.Thrift
{
    public class TAsyncSystemSocket : TAsyncSocket
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



        private static LimitedConcurrencyLevelTaskScheduler s_IOReadTaskSchduller = new LimitedConcurrencyLevelTaskScheduler(1);
        private static TaskFactory s_IoReadTaskFactory = new TaskFactory(s_IOReadTaskSchduller);

        private static LimitedConcurrencyLevelTaskScheduler s_IOWriteTaskSchduller = new LimitedConcurrencyLevelTaskScheduler(1);
        private static TaskFactory s_IoWriteTaskFactory = new TaskFactory(s_IOReadTaskSchduller);
                                                                                                 
        public event EventHandler<SocketAsyncEventArgs> connectHandler = null;

        // memory stream for write cache.


        //private ConcurrentDictionary<UInt32, SocketReceiveContext> m_ReciveContentBuffer = new ConcurrentDictionary<UInt32, SocketReceiveContext>();
       
        //private ConcurrentQueue<MemoryStream> m_PendingStream = new ConcurrentQueue<MemoryStream>();

       


        // constructor
        public TAsyncSystemSocket(string host, int port)
            : base(host, port, 0)
        {
        }

        // constructor
        public TAsyncSystemSocket(string host, int port, int timeout)
            : base(host, port, timeout)
        {
           

            InitSocket();
        }

        private void InitSocket()
        {
            // Create a stream-based, TCP socket using the InterNetwork Address Family.
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
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

       
        public override Task<bool> OpenAsyncTask()
        {
            
            TaskCompletionSource<bool> retSource = new TaskCompletionSource<bool>();
            if (this.IsOpen)
            {
                throw new TTransportException(TTransportException.ExceptionType.AlreadyOpen, "Socket already connected");
            }

            if (String.IsNullOrEmpty(Host))
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

           
            {
                // Create DnsEndPoint. The hostName and port are passed in to this method.
                DnsEndPoint hostEntry = new DnsEndPoint(this.host, this.port);

                // Create a SocketAsyncEventArgs object to be used in the connection request
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = hostEntry;

                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
                {
                    if (connectHandler != null)
                    {
                        connectHandler(this, e);
                    }
                    if (e.SocketError == SocketError.Success)
                        retSource.SetResult(true);
                    else
                    {
                        retSource.SetException(new TTransportException(e.SocketError.ToString()));       
                    }
                });


                // Make an asynchronous Connect request over the socket
                if(!socket.ConnectAsync(socketEventArg))
                {
                    if (socketEventArg.SocketError == SocketError.Success)
                        retSource.SetResult(true);
                    else
                    {
                        
                        retSource.SetException(new TTransportException(socketEventArg.SocketError.ToString()));

                    }
                }
            }

            return retSource.Task;
        }
        /*
        public async Task OpenAsync()
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
            catch(TTransportException ex)
            {
                throw ex;
            }
            if (complete == timeoutTask)
            {
                throw new TTransportException("Connection Timeout");
            }
        }
*/
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

        private object m_SocketReadLock = new object();

        public override Task<MemoryStream> UnFrame(UInt32 seqID)
        {
           
            EventHandler<SocketAsyncEventArgs> handler = (o, ea) =>
            {
                SocketReceiveContext receiveContext = (SocketReceiveContext)ea.UserToken;


                if (ea.SocketError == SocketError.Success)
                {
                    
                   
                }
                else
                {
                    LogService.Logger.Log(LogService.LogType.LT_ERROR, ea.ToString());
                }

                receiveContext.m_ReadCompleteEvent.Set();

              

            };

            TaskCompletionSource<MemoryStream> retSource = new TaskCompletionSource<MemoryStream>();
                
            Task.Factory.StartNew(() =>
            {
                SocketReceiveContext headerReceiveContext = new SocketReceiveContext();
                headerReceiveContext.m_SeqID = seqID;


                SocketAsyncEventArgs headerEventArg = new SocketAsyncEventArgs();
                MemoryStream headerStream = new MemoryStream(12);
                headerStream.SetLength(4);
                headerEventArg.SetBuffer(headerStream.GetBuffer(), 0, 4);
                headerEventArg.UserToken = headerReceiveContext;
                headerEventArg.RemoteEndPoint = socket.RemoteEndPoint;

                headerEventArg.Completed += handler;

                lock(m_SocketReadLock)
                {

                    socket.ReceiveAsync(headerEventArg);
                    if (!headerReceiveContext.m_ReadCompleteEvent.WaitOne(Timeout))
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
                   
                    MemoryStream content = new MemoryStream((int)frameSize);
                    content.SetLength(frameSize);
                    int byteTransferred = 0;
                    int remaining = (int)frameSize;

                    while (remaining > 0)
                    {
                        SocketReceiveContext contentReceiveContext = new SocketReceiveContext();
                        contentReceiveContext.m_SeqID = seqID;
                        SocketAsyncEventArgs contentEventArg = new SocketAsyncEventArgs();
                        contentEventArg.UserToken = contentReceiveContext;
                        contentEventArg.SetBuffer(content.GetBuffer(), byteTransferred, (int)remaining);
                        contentEventArg.RemoteEndPoint = socket.RemoteEndPoint;
                        contentEventArg.Completed += handler;

                        socket.ReceiveAsync(contentEventArg);

                        if(!contentReceiveContext.m_ReadCompleteEvent.WaitOne(Timeout))
                        {
                            retSource.SetException(new TTransportException(TTransportException.ExceptionType.TimedOut, "Unexpected timeout"));
                            return;
                        }
                        byteTransferred += contentEventArg.BytesTransferred;
                        remaining -= contentEventArg.BytesTransferred;

                    }
                   
                    retSource.SetResult(content);



                }

            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);


            return retSource.Task;
        }

       

        private void beginFlush_Completed(object sender, SocketAsyncEventArgs e)
        {
            SocketFlushAsyncResult flushAsyncResult = e.UserToken as SocketFlushAsyncResult;
            flushAsyncResult.UpdateStatusToComplete();
            flushAsyncResult.NotifyCallbackWhenAvailable();

            if (e.SocketError != SocketError.Success)
            {
                throw new TTransportException(TTransportException.ExceptionType.Unknown, e.SocketError.ToString());
            }
        }



        public override IAsyncResult BeginFlush(AsyncCallback callback, object state, UInt32 seqID)
        {
            // Extract request and reset buffer
            byte[] data = outputStream.Value.ToArray();
            m_StreamPool.Enqueue(outputStream.Value);
            outputStream.Value = null;

            SocketFlushAsyncResult flushAsyncResult = new SocketFlushAsyncResult(callback, state);
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


