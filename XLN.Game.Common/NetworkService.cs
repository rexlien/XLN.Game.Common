using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Thrift.Protocol;
using Thrift.Transport;
using XLN.Game.Common.Config;
using XLN.Game.Common.Thrift;

namespace XLN.Game.Common
{
    [GuidAttribute("35BE04C7-B23D-4D31-A678-E0D112C753CD")]
    public class NetworkService : IService
    {
        public NetworkService()
        {
            m_ConnectedAction += OnConnected;
            m_DisConnectedAction += OnDisconnected;
            m_ConnectionErrorAction += OnConnectError;

            m_ServerConfig = ApplicationContext.AppConig.AppNetworks.ServerItems.Find(pred => pred.Name == "GameServer");
        }

        public class NetworkServieException : Exception
        {
            public NetworkServieException(string message)
                : base(message)
            {
            }

            public int ErrorCode;
        }

        public class ConnectionResult
        {
            public bool Result;
            public Exception Exception;

            public ConnectionResult(bool result, Exception exception)
            {
                Result = result;
                Exception = exception;
            }
        }

        private Action<TFramedTransport> m_ConnectedAction;
        private Action m_DisConnectedAction;
        private Action<Exception> m_ConnectionErrorAction;
        protected AppConfig.Server m_ServerConfig;
        ///private bool m_Init = false;
        /// 
        public override bool OnInit()
        {
           
            return base.OnInit();
        }

        public virtual Task<ConnectionResult> StartConnect()
        {
            if(m_ServerConfig == null)
            {
                return Task.FromResult(new ConnectionResult(false, new NetworkServieException("Server config not set")));
            }

            if(m_ConnectionTask != null && !m_ConnectionTask.IsCompleted)
            {
                LogService.Logger.Log(LogService.LogType.LT_WARNING, "connection task not complete yet");
                return Task.FromResult(new ConnectionResult(false, new NetworkServieException("connection task not complete yet")));    
            }

            var task = Task.Factory.StartNew(() =>
            {
                TSocket socket = new TSocket(m_ServerConfig.IP, m_ServerConfig.Port);

                //TODO this also set the recevie timeout..which could be not we wanted
                socket.Timeout = 10000;
                TFramedTransport transport = new TFramedTransport(socket);
                try
                {
                    transport.Open();
                }
                catch (TTransportException e)
                {
                    LogService.Logger.Log(LogService.LogType.LT_ERROR, e.Message);

                    transport.Close();
                    transport = null;
                    socket = null;
                    throw e;

                }
                return transport;
            });

            m_ConnectionTask = task.ContinueWith((t)=>{
                
                if (t.IsCompleted)
                {
                    if (t.IsFaulted)
                    {
                        m_ConnectionErrorAction(t.Exception);
                        return new ConnectionResult(false, t.Exception);
                        //throw t.Exception;
                    }
                    else if(t.IsCanceled)
                    {
                        return new ConnectionResult(false, new NetworkServieException("connection task Cancelled"));
                    }
                    else
                    {
                        var transport = t.Result;

                        m_Transport = transport;
                        TProtocol protocol = new TBinaryProtocol(m_Transport);
                        Thrift.NetworkService.Client newClient = new Thrift.NetworkService.Client(protocol);
                        lock(m_NetworkServiceClientLock)
                        {
                            m_NetworkServiceClient = newClient;
                        }
                        m_ConnectedAction(m_Transport);
                        return new ConnectionResult(true, null);

                    }
                }
                return new ConnectionResult(false, null);;

            
            }, ApplicationContext.MainScheduler);

            return m_ConnectionTask;       
           
                

        }

        private void ClearConnection()
        {
            //m_HeartbeatTaskCancelOperation.Cancel();
            lock (m_NetworkServiceClientLock)
            {
                m_NetworkServiceClient = null;
            }

            if (m_Transport != null)
            {
                m_Transport.Close();
                m_Transport = null;
            }

        }

        public override bool OnDestroy()
        {
            m_ConnectionTask = null;
            m_HeartbeatTask = null;
            ClearConnection();
            return true;
        }


        protected virtual void OnConnected(TFramedTransport transport)
        {
            StartHeartbeat();
        }

        protected virtual void OnConnectError(Exception e)
        {
            
        }

        protected virtual void OnDisconnected()
        {


        }

        public void RegisterDisconnectedAction(Action action)
        {
            m_DisConnectedAction += action;
        }

        protected void StartHeartbeat()
        {
            if (m_HeartbeatTask != null)
                return;

            //CancellationToken token = m_HeartbeatTaskCancelOperation.Token;

            //TODO maybe a scheduller in dedicated thread
            //TaskFactory factory = new TaskFactory(new TaskScheduler(null));
            var task = Task.Factory.StartNew(() =>
            {
                
                StartHeartbeatAsync();

            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
                                            
        }

        protected async void StartHeartbeatAsync()
        {
            Thrift.NetworkService.Client networkClient = null;
            while (true)
            {
                //if(token.IsCancellationRequested)
                //{
                //    await Task.Delay(-1, m_HeartbeatStopperCancelOperation.Token);
                //}
                
                lock (m_NetworkServiceClientLock)
                {
                    networkClient = NetworkServiceClient;
                }
               
                try
                {
                    if (networkClient != null)
                    {
                        //long millisecond = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                        //DateTimeOffset.
                        TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                        long timestamp = (long)t.TotalSeconds;
                        networkClient.heartbeat(timestamp);
                    }
                }
                catch(System.IO.IOException ex)
                {
                    LogService.Logger.Log(LogService.LogType.LT_WARNING, ex.ToString());
                    var t = Task.Factory.StartNew( () =>
                    { 
                        ClearConnection();
                        m_DisConnectedAction();
                        //OnDisconnected();

                    }, CancellationToken.None, TaskCreationOptions.None, ApplicationContext.MainScheduler);
                }

                //send heartbeat every 10 seconds
                await Task.Delay(10000);
            }

        }

        protected Thrift.NetworkService.Client m_NetworkServiceClient;
        public Thrift.NetworkService.Client NetworkServiceClient
        {
            get
            {
                return m_NetworkServiceClient;
            }
        }

        protected object m_NetworkServiceClientLock = new object();

        private TFramedTransport m_Transport;
        //private CancellationTokenSource m_HeartbeatTaskCancelOperation = new CancellationTokenSource();
        //private CancellationTokenSource m_HeartbeatStopperCancelOperation = new CancellationTokenSource();
        private Task<ConnectionResult> m_ConnectionTask;
        private Task m_HeartbeatTask;

    }
}
