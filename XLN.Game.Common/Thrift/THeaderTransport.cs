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
 */
using System;
using System.IO;
using System.Threading;
using Thrift.Protocol;
using System.Collections.Generic;
using System.Net;
using XLN.Game.Common.Utils;
using XLN.Game.Common.Extension;
using XLN.Game.Common;
using System.Threading.Tasks;

namespace Thrift.Transport
{
    public class THeaderTransport : TTransport, IDisposable
    {

        public enum CLIENT_TYPE
        {
            THRIFT_HEADER_CLIENT_TYPE = 0,
            THRIFT_FRAMED_DEPRECATED = 1,
            THRIFT_UNFRAMED_DEPRECATED = 2,
            THRIFT_HTTP_SERVER_TYPE = 3,
            THRIFT_HTTP_CLIENT_TYPE = 4,
            THRIFT_FRAMED_COMPACT = 5,
            THRIFT_HEADER_SASL_CLIENT_TYPE = 6,
            THRIFT_HTTP_GET_CLIENT_TYPE = 7,
            THRIFT_UNKNOWN_CLIENT_TYPE = 8,
            THRIFT_UNFRAMED_COMPACT_DEPRECATED = 9,
        };

        public enum HEADER_FLAGS
        {
            HEADER_FLAG_SUPPORT_OUT_OF_ORDER = 0x01,
            // Set for reverse messages (server->client requests, client->server replies)
            HEADER_FLAG_DUPLEX_REVERSE = 0x08,
            HEADER_FLAG_SASL = 0x10,
        };

        static readonly UInt32 HEADER_MAGIC = 0x0FFF0000;
        static readonly UInt32 HEADER_MASK = 0xFFFF0000;
        static readonly UInt32 FLAGS_MASK = 0x0000FFFF;

        static readonly UInt32 MAX_FRAME_SIZE = 0x3FFFFFFF;
        static readonly UInt32 BIG_FRAME_MAGIC = 0x42494746;
        private Int16 m_ProtocolID;
        private UInt16 m_ClientType;
        private UInt32 m_SeqID;
        private UInt16 m_Flags;

        private Int16 m_InProtocolID;

        private static ThreadLocal<UInt32> s_CurSequence = new ThreadLocal<UInt32>();
        static Int32 s_SequenceCounter;

        private readonly ThreadLocal<MemoryStream> writeBuffer = new ThreadLocal<MemoryStream>( ()=> 
        { 
            MemoryStream stream = new MemoryStream(1024);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;

        });
        private readonly ThreadLocal<MemoryStream> readBuffer = new ThreadLocal<MemoryStream>(() =>
       {
           MemoryStream stream = new MemoryStream(1024);
           //stream.SetLength(HeaderSize);
           stream.Seek(0, SeekOrigin.End);
           return stream;

       });
        //private const int HeaderSize = 4;
        //private readonly ThreadLocal<byte[]> headerBuf = new ThreadLocal<byte[]>( () => { return new byte[HeaderSize]; });
        private readonly TAsyncSocket m_AsyncSocket;

        private ThreadLocal<Dictionary<string, string>> m_WriteHeaders = new ThreadLocal<Dictionary<string, string>>(() => { return new Dictionary<string, string>(); });
        private ThreadLocal<Dictionary<string, string>> m_ReadHeaders = new ThreadLocal<Dictionary<string, string>>(() => { return new Dictionary<string, string>(); });
       
       
        public THeaderTransport(TAsyncSocket transport)
        {
            if (transport == null)
                throw new ArgumentNullException("transport");
            this.m_AsyncSocket = transport;
            m_ClientType = (ushort)CLIENT_TYPE.THRIFT_HEADER_CLIENT_TYPE;
            m_Flags = (ushort)HEADER_FLAGS.HEADER_FLAG_SUPPORT_OUT_OF_ORDER;
           
        }

        public void SetHeader(string header, string value)
        {
            m_WriteHeaders.Value.Add(header, value);
        }

        public void ClearHeader()
        {
            m_WriteHeaders.Value.Clear();
        }

        protected void WriteString(byte[] outByte, string str)
        {
            
        }

        protected void ReadSTring(string str, byte[] inByte)
        {
            
        }



        public override void Open()
        {
            CheckNotDisposed();
            m_AsyncSocket.Open();
        }

        public override bool IsOpen
        {
            get
            {
                // We can legitimately throw here but be nice a bit.
                // CheckNotDisposed();
                return !_IsDisposed && m_AsyncSocket.IsOpen;
            }
        }


        public override void Close()
        {
            CheckNotDisposed();
            m_AsyncSocket.Close();
        }

        public override int Read(byte[] buf, int off, int len)
        {
            CheckNotDisposed();
            ValidateBufferArgs(buf, off, len);
            if (!IsOpen)
                throw new TTransportException(TTransportException.ExceptionType.NotOpen);
            int got = readBuffer.Value.Read(buf, off, len);
            if (got > 0)
            {
                return got;
            }

            // Read another frame of data
            ReadFrame();

            return readBuffer.Value.Read(buf, off, len);
        }

        private object m_ReadLock = new object();

        private void ReadFrame()
        {
            //TAsyncSocket.SocketReceiveContext receiveContext = new TAsyncSocket.SocketReceiveContext();
            //receiveContext.m_SeqID = s_CurSequence.Value;

            //transport.ReadAll(headerBuf.Value, 0, HeaderSize);
            //MemoryStream headerStream = new MemoryStream(1024);
            //headerStream.SetLength(4);
            //lock (m_ReadLock)
            //{
            //    m_AsyncSocket.ReadWithContext(headerStream.GetBuffer(), 0, 4, receiveContext);
            //    uint frameSize = 0;
            //    using (BinaryReader reader = new BinaryReader(headerStream, System.Text.Encoding.Default, true))
            //    {
            //        frameSize = reader.ntoh32();
            //        if (frameSize > MAX_FRAME_SIZE)
            //        {
            //            //TODO 
            //            headerStream.SetLength(12);
            //            m_AsyncSocket.ReadWithContext(headerStream.GetBuffer(), 4, 8, receiveContext);
            //        }
            //        else
            //        {

            //        }

            //    }
            //    //headerStream.Seek();


            //    readBuffer.Value.SetLength(frameSize);
            //    readBuffer.Value.Seek(0, SeekOrigin.Begin);
            //    byte[] buff = readBuffer.Value.GetBuffer();
            //    m_AsyncSocket.ReadWithContext(buff, 0, (Int32)frameSize, receiveContext);
            //}

            MemoryStream content = m_AsyncSocket.UnFrame(s_CurSequence.Value).Result;
            using (BinaryReader reader = new BinaryReader(content, System.Text.Encoding.Default, true))
            {
                UInt16 magic = reader.ntoh16();
                UInt16 flags = reader.ntoh16();
                UInt32 seqID = reader.ntoh32();

                LogService.Logger.Log(LogService.LogType.LT_DEBUG, "sent SeqID: "+ s_CurSequence.Value + " received SeqID" + seqID );

                UInt16 headerSize = reader.ntoh16();

                long headerStart = content.Position;
                long headerSizeByte = headerSize * 4;
                long headerEnd = headerStart + headerSizeByte;
                m_InProtocolID = (Int16)reader.ntohVarint32();//Varint.ReadVarint32(reader);

                UInt16 transofromSize = (UInt16)Varint.ReadVarint32(reader);
                for (int i = 0; i < transofromSize; i++)
                {   //TODO support transform
                    UInt32 transformID = (UInt32)reader.ntohVarint32();
                }

                while(content.Position != headerEnd)
                {
                    UInt32 infoID = (UInt32)reader.ntohVarint32();
                    if(infoID == 0)
                    {
                        break;
                    }

                    //TODO support header
                }

                //end of header
                content.Seek(headerEnd, SeekOrigin.Begin);

                //this may not be for our sequence
                TaskCompletionSource<TAsyncSocket.PendingBufferEntry> taskSource = null;
                m_AsyncSocket.PendingBuffers.TryGetValue(seqID, out taskSource);
                if (taskSource != null)
                {
                    TAsyncSocket.PendingBufferEntry entry = new TAsyncSocket.PendingBufferEntry();
                    entry.m_ContentBuffer = content;
                    taskSource.SetResult(entry);
                }
           
            }


            TaskCompletionSource<TAsyncSocket.PendingBufferEntry> myTaskSource = null;
            m_AsyncSocket.PendingBuffers.TryRemove(s_CurSequence.Value, out myTaskSource);
            if(myTaskSource != null)
            {
                var task = myTaskSource.Task;
                MemoryStream stream = task.Result.m_ContentBuffer;
                readBuffer.Value = stream;

                //Task<Aysnc> myTaskSource.;

            }
            else
            {
                throw new TTransportException("Unexpected error occurs");

            }

        }

        public override void Write(byte[] buf, int off, int len)
        {
            CheckNotDisposed();
            ValidateBufferArgs(buf, off, len);
            if (!IsOpen)
                throw new TTransportException(TTransportException.ExceptionType.NotOpen);
            if (writeBuffer.Value.Length + (long)len > (long)int.MaxValue)
                throw new TTransportException("Maximum send size limited reached");
            writeBuffer.Value.Write(buf, off, len);
        }

        public override void Flush()
        {

            CheckNotDisposed();
            if (!IsOpen)
                throw new TTransportException(TTransportException.ExceptionType.NotOpen);
            if (writeBuffer.Value.Length < 0)
                throw new System.InvalidOperationException();

            //MemoryStream frameHeader = new MemoryStream(4);
            MemoryStream frame = new MemoryStream(1024);
            //skip first 8 for framesize
            long frameSize = 0;
            frame.SetLength(12);
            frame.Seek(12, SeekOrigin.Begin);

            using (BinaryWriter writer = new BinaryWriter(frame, System.Text.Encoding.Default, true))
            {
                long commonHeaderOff = frame.Position;
                UInt16 magicN = (UInt16)(HEADER_MAGIC >> 16);
                writer.Write(IPAddress.HostToNetworkOrder((short)magicN));
                UInt16 flagsN = 0;

                if (m_ClientType == (ushort)CLIENT_TYPE.THRIFT_HEADER_SASL_CLIENT_TYPE)
                {
                    flagsN = (ushort)IPAddress.HostToNetworkOrder((short)(m_Flags | (ushort)(HEADER_FLAGS.HEADER_FLAG_SASL)));
                }
                else
                {
                    flagsN = (ushort)IPAddress.HostToNetworkOrder((short)m_Flags);
                }
                writer.Write(flagsN);

                UInt32 currentSeqID = (UInt32)Interlocked.Increment(ref s_SequenceCounter);
                s_CurSequence.Value = currentSeqID;
                writer.Write((UInt32)IPAddress.HostToNetworkOrder((int)currentSeqID));

                //header size, later to fixed up
                long headerOff = frame.Position;
                UInt16 headerSizeN = 0;
                writer.Write(headerSizeN);

                long commonHeaderSize = frame.Position - commonHeaderOff;


                Varint.WriteVarint32(writer, (uint)m_ProtocolID);

                //no transform support
                Varint.WriteVarint32(writer, 0);

                //no info header support

                long headerSize = frame.Position - headerOff - 2;
                long padding = 4 - headerSize % 4;
                for (int i = 0; i < padding; i++)
                {
                    writer.Write((byte)0);
                }
                headerSize += padding;
                headerSizeN = (UInt16)(headerSize / 4);
                frameSize = commonHeaderSize + headerSize + writeBuffer.Value.Length;


                if(frameSize > MAX_FRAME_SIZE )
                {
                    UInt32 szNbo = (UInt32)IPAddress.HostToNetworkOrder((int)BIG_FRAME_MAGIC);
                    frame.Seek(0, SeekOrigin.Begin);
                    writer.Write(szNbo);
                    long largeFrameSize = IPAddress.HostToNetworkOrder(frameSize);
                    writer.Write(szNbo);

                }
                else
                {
                    frame.Seek(8, SeekOrigin.Begin);
                    writer.Write((UInt32)IPAddress.HostToNetworkOrder((int)frameSize));
                }

                frame.Seek(headerOff, SeekOrigin.Begin);
                writer.Write((UInt16)IPAddress.HostToNetworkOrder((short)headerSizeN));


                //frame

            }

            // Send the entire message at once
            if (frameSize > MAX_FRAME_SIZE)
                m_AsyncSocket.Write(frame.GetBuffer(), 0, (int)frame.Length);
            else
                m_AsyncSocket.Write(frame.GetBuffer(), 8, (int)frame.Length - 8);
            
            m_AsyncSocket.Write(writeBuffer.Value.GetBuffer(), 0, (int)writeBuffer.Value.Length);

            ResetWriteBuffer();

            m_AsyncSocket.Flush();
        }


        public override IAsyncResult BeginFlush(AsyncCallback callback, object state)
        {
            Flush();
            return m_AsyncSocket.BeginFlush(callback, state, s_CurSequence.Value);
        }

       public override void EndFlush(IAsyncResult asyncResult)
        {
            TAsyncSocket.FlushAsyncResult result = (TAsyncSocket.FlushAsyncResult)asyncResult;
            s_CurSequence.Value = result.SeqID;
            m_AsyncSocket.PendingBuffers.TryAdd(result.SeqID, new TaskCompletionSource<TAsyncSocket.PendingBufferEntry>());
                         
            m_AsyncSocket.EndFlush(asyncResult);
        }

        private void ResetWriteBuffer()
        {
            // Reserve space for message header to be put right before sending it out
            writeBuffer.Value.SetLength(0);
            writeBuffer.Value.Seek(0, SeekOrigin.End);
        }


        public void BeginAddHeader()
        {
            
        }


        private void CheckNotDisposed()
        {
            if (_IsDisposed)
                throw new ObjectDisposedException("TFramedTransport");
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
                    if (readBuffer != null)
                        readBuffer.Dispose();
                    if (writeBuffer != null)
                        writeBuffer.Dispose();
                    if (m_AsyncSocket != null)
                        m_AsyncSocket.Dispose();
                }
            }
            _IsDisposed = true;
        }
        #endregion


        public int SeqID { get => SeqID; set => SeqID = value; }
        public short ProtocolID { get => m_ProtocolID; set => m_ProtocolID = value; }
        public ushort ClientType { get => m_ClientType; set => m_ClientType = value; }
        public ushort Flags { get => m_Flags; set => m_Flags = value; }
    }
}
