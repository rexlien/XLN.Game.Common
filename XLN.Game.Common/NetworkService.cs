using System;
using System.Net.Sockets;
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

        private Action<TTransport> m_ConnectedAction;
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
                TAsyncSocket socket = new TAsyncSocket(m_ServerConfig.IP, m_ServerConfig.Port);
                socket.Timeout = 10000;
                TTransport transport = new THeaderTransport(socket);
                AutoResetEvent barrier = new AutoResetEvent(false);

                TTransportException tException = new TTransportException("Connection Timeout");

                socket.connectHandler += (object sender, System.Net.Sockets.SocketAsyncEventArgs e) => 
                {
                    
                    if(e.SocketError == SocketError.Success)
                    {
                         tException = null;
                    }
                    else 
                    {
                        tException = new TTransportException(e.SocketError.ToString());
                    }
                    barrier.Set();
                };
                //TODO this also set the recevie timeout..which could be not we wanted

                transport.Open();
                barrier.WaitOne(10000);
                TTransportException ex = tException;
                if(ex != null)
                {
                    LogService.Logger.Log(LogService.LogType.LT_ERROR, ex.Message);
                    transport.Close();
                    transport = null;
                    throw ex;
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
                        TProtocol protocol = new THeaderProtocol((THeaderTransport)m_Transport, THeaderProtocol.PROTOCOL_TYPES.T_BINARY_PROTOCOL);
                        m_NetworkServiceClient = new Thrift.NetworkService.Client(protocol);;
                        m_ActorServiceClient = new Thrift.ActorService.Client(protocol);

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

            m_NetworkServiceClient = null;
            m_ActorServiceClient = null;

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


        protected virtual void OnConnected(TTransport transport)
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

        public void RegisterConnectedAction(Action<TTransport> action)
        {
            m_ConnectedAction += action;
        }

        protected void StartHeartbeat()
        {
            if (m_HeartbeatTask != null)
                return;
            
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
             
                networkClient = NetworkServiceClient;
                  
                try
                {
                    if (networkClient != null)
                    {
                        //long millisecond = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                        //DateTimeOffset.
                        TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                        long timestamp = (long)t.TotalSeconds;

                        //LogService.Logger.Log(LogService.LogType.LT_DEBUG, "heartbeat await start");
                        long local = await Task.Factory.FromAsync(networkClient.send_heartbeat(null, null, timestamp), (asyncResult) =>
                        {
                            //LogService.Logger.Log(LogService.LogType.LT_DEBUG, "heartbeat end call begin ");
                            long ret = networkClient.End_heartbeat(asyncResult);
                            //LogService.Logger.Log(LogService.LogType.LT_DEBUG, "heartbeat end call end ");
                            return ret;
                        
                        });//networkClient.End_heartbeat);
                        //LogService.Logger.Log(LogService.LogType.LT_DEBUG, "heartbeat await end resul " + local.ToString());

                        //long local = await ClientSend<XLN.Game.Common.Thrift.NetworkService.Client, long>(
                        //    (client) => { return Task.Factory.FromAsync(client.send_heartbeat(null, null, 1000), client.End_heartbeat); }, null, null, 1000);
                        //LogService.Logger.Log(LogService.LogType.LT_DEBUG, "heartbeat await end resul " + local.ToString());
            
                    }
                }
                catch(TTransportException ex)
                {
                    LogService.Logger.Log(LogService.LogType.LT_WARNING, ex.ToString());
                    await Task.Factory.StartNew( () =>
                    { 
                        ClearConnection();
                        m_DisConnectedAction();


                    }, CancellationToken.None, TaskCreationOptions.None, ApplicationContext.MainScheduler);
                }
                catch(Exception ex)
                {
                    LogService.Logger.Log(LogService.LogType.LT_WARNING, ex.ToString());
                    //throw ex;
                }

                //send heartbeat every 10 seconds
                await Task.Delay(2000);
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

        protected Thrift.ActorService.Client m_ActorServiceClient;
        public Thrift.ActorService.Client ActorServiceClient
        {
            get
            {
                return m_ActorServiceClient;
            }
        }

       
        public Task<R> ClientSend<Client, R>(Func<Client, Task<R>> thriftAction, Action<Task> onSuccess,
            Action<Task> onFail, int timeoutMillis)
        {

            var task = Task.Factory.StartNew(async delegate
           {
               try
               {
                    TProtocol protocol = new THeaderProtocol((THeaderTransport)m_Transport, THeaderProtocol.PROTOCOL_TYPES.T_BINARY_PROTOCOL);
                   Client client = XLN.Game.Common.Utils.ClassUtils.CreateInstance<Client>(protocol);
                    return await thriftAction(client);
                  
                                      //Task.FromResult(default(R));
                                      //if (onPostTransport != null)
                                      //    onPostTransport(task);
                }
               catch (Exception e)
               {
                   throw e;
               }


            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();
           
            task .ContinueWith((t) =>
                {
                    if (t.Exception == null)
                    {
                        if (onSuccess != null)
                            onSuccess(t);
                    }
                    else
                    {
                        
                        if (onFail != null)
                        {
                            onFail(t);
                        }
                    }

                }, ApplicationContext.MainScheduler);

            return task;

        }

        public Task ClientSend<Client>(Func<Client, Task> thriftAction, Action<Task> onSuccess,
            Action<Task> onFail, int timeoutMillis)
        {
            Task task = null;
            task = Task.Factory.StartNew(async () =>
            {
                try
                {
                    TProtocol protocol = new THeaderProtocol((THeaderTransport)m_Transport, THeaderProtocol.PROTOCOL_TYPES.T_BINARY_PROTOCOL);
                    Client client = XLN.Game.Common.Utils.ClassUtils.CreateInstance<Client>(protocol);
                    await thriftAction(client);

                    //if (onPostTransport != null)
                    //    onPostTransport(task);
                }
                catch (Exception e)
                {
                    throw e;
                }

            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                .ContinueWith((t) =>
                {
                    if (t.Exception == null)
                    {
                        if (onSuccess != null)
                            onSuccess(t);
                    }
                    else
                    {

                        if (onFail != null)
                        {
                            onFail(t);
                        }
                    }

                }, ApplicationContext.MainScheduler);

            return task;

        }



        private TTransport m_Transport;
        private Task<ConnectionResult> m_ConnectionTask;
        private Task m_HeartbeatTask;

    }
}
