using System;
using System.IO;
using System.Threading.Tasks;
using Thrift.Transport;
using XLN.Game.Common;

namespace XLN.Game.Unity.Thrift
{
    public class TAsyncWebSocket : Common.Thrift.TAsyncSocket
    {

        private WebGLSocket m_Socket;
        private string m_URL;

        public TAsyncWebSocket(string host, int port) : this(host, port, 0)
        {

        }

        public TAsyncWebSocket(string host, int port, int timeout) : base(host, port, timeout)
        {

            Uri uri = new Uri(host);
            if (uri.Scheme == "https")
                m_URL = "wss://" + uri.Host + ":" + port;
            else
                m_URL = "ws://" + uri.Host + ":" + port;

            LogService.Logger.Log(LogService.LogType.LT_DEBUG, "websocket url:" + m_URL);
        }

        public override bool IsOpen
        {
            get { return (m_Socket != null) && m_Socket.Connected; }
        }

        public override IAsyncResult BeginFlush(AsyncCallback callback, object state, uint seqID)
        {
            byte[] data = outputStream.Value.ToArray();
            m_StreamPool.Enqueue(outputStream.Value);
            outputStream.Value = null;

            SocketFlushAsyncResult flushAsyncResult = new SocketFlushAsyncResult(callback, state);
            flushAsyncResult.SeqID = seqID;

            m_Socket.SendAsync(data);

            flushAsyncResult.UpdateStatusToComplete();
            flushAsyncResult.NotifyCallbackWhenAvailable();

            LogService.Logger.Log(LogService.LogType.LT_DEBUG, "socket flushed");

            return flushAsyncResult;
        }

        public override async Task OpenAsync()
        {
            TaskCompletionSource<bool> retSource = new TaskCompletionSource<bool>();

            Task<bool> openTask = OpenAsyncTask();
            Task complete = null;
            try
            {
                complete = await Task.WhenAny(openTask);
                await complete;
            }
            catch (TTransportException ex)
            {
                throw ex;
            }
          
        }

		public override void Flush()
		{
            byte[] data = outputStream.Value.ToArray();
            m_StreamPool.Enqueue(outputStream.Value);
            outputStream.Value = null;

            m_Socket.SendAsync(data);

            base.Flush();

            LogService.Logger.Log(LogService.LogType.LT_DEBUG, "socket flushed");

		}

		public override Task<bool> OpenAsyncTask()
        {
            if(IsOpen)
                throw new TTransportException(TTransportException.ExceptionType.AlreadyOpen, "Socket already connected");
            
            m_Socket = new WebGLSocket();
            return m_Socket.ConnectAsync(m_URL);

        }



        public override Task<MemoryStream> UnFrame(uint seqID)
        {
            throw new NotImplementedException();
        }
    }
}
