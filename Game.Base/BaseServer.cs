using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Net;
using System.Collections;

namespace Game.Base
{
    /// <summary>
    /// Base class for a server using overlapped socket IO
    /// </summary>
    public class BaseServer
    {
        /// <summary>
        /// Defines a logger for this class
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Defines send buffer size.
        /// </summary>
        private static readonly int SEND_BUFF_SIZE = 16 * 1024;

        /// <summary>
        /// Hash table of clients
        /// </summary>
        protected readonly HybridDictionary _clients = new HybridDictionary();

        /// <summary>
        /// Socket that receives connections.
        /// </summary>
        protected Socket _linstener;

        protected SocketAsyncEventArgs ac_event;

        /// <summary>
        /// Constructor task a server conifuration as parameter
        /// </summary>
        /// <param name="config"></param>
        public BaseServer()
        {
            ac_event = new SocketAsyncEventArgs();
            ac_event.Completed += AcceptAsyncCompleted;
        }

        /// <summary>
        /// Returns the number of clients currently connected to the server
        /// </summary>
        public int ClientCount
        {
            get
            {
                return _clients.Count;
            }
        }

        /// <summary>
        /// Begins a asychnorous accept call.
        /// </summary>
        private void AcceptAsync()
        {
            try
            {
                if (_linstener != null)
                {
                    SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                    e.Completed += AcceptAsyncCompleted;
                    _linstener.AcceptAsync(e);
                }
            }
            catch (Exception ex)
            {
                log.Error("AcceptAsync is error!" , ex);
            }
        }
      
        /// <summary>
        /// Accepts complete event's callback funciton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket sock = null;
            try
            {
                sock = e.AcceptSocket;
                sock.SendBufferSize = SEND_BUFF_SIZE;

                BaseClient client = GetNewClient();

                //TrieuLSL

                try
                {
                    if (log.IsInfoEnabled)
                    {
                        string ip = sock.Connected ? sock.RemoteEndPoint.ToString() : "socket disconnected";
                        log.Info("Incoming connection from " + ip);
                    }

                    lock (_clients.SyncRoot)
                    {
                        
                        _clients.Add(client, client);//Add the client instance to a hy dictionary.
                        client.Disconnected += client_Disconnected;
                    }

                    client.Connect(sock);
                    client.ReceiveAsync();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("create client failed:{0}", ex);
                    client.Disconnect();
                }
            }
            catch
            {
                if (sock != null) // don't leave the socket open on exception
                    try { sock.Close(); }
                    catch { }
            }
            finally
            {
                e.Dispose();
                AcceptAsync();
            }
        }

        private void client_Disconnected(BaseClient client)
        {
            client.Disconnected -= client_Disconnected;
            RemoveClient(client);
        }

        /// <summary>
        /// Creates a new client object
        /// </summary>
        /// <returns>A new client object</returns>
        protected virtual BaseClient GetNewClient()
        {
            return new BaseClient(new byte[2048], new byte[2048]);
        }

        /// <summary>
        /// Initializes and binds the socket, doesn't listen yet!
        /// </summary>
        /// <returns>true if bound</returns>
        public virtual bool InitSocket(IPAddress ip, int port)
        {
            try
            {
                _linstener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _linstener.Bind(new IPEndPoint(ip, port));
            }
            catch (Exception e)
            {
                log.Error("InitSocket", e);

                return false;
            }
            return true;
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        /// <returns>True if the server was successfully started</returns>
        public virtual bool Start()
        {
            //Test if we have a valid port yet
            //if not try  binding.
            if (_linstener == null)
                return false;

            try
            {
                _linstener.Listen(100);

                AcceptAsync();

                if (log.IsDebugEnabled)
                    log.Debug("Server is now listening to incoming connections!");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Start", e);
                if (_linstener != null)
                    _linstener.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public virtual void Stop()
        {
            log.Debug("Stopping server! - Entering method");
            try
            {
                if (_linstener != null)
                {
                    Socket socket = _linstener;
                    _linstener = null;
                    socket.Close();
                    log.Debug("Server is no longer listening for incoming connections!");
                }
            }
            catch (Exception e)
            {
                log.Error("Stop", e);
            }

            if (_clients != null)
            {
                lock (_clients.SyncRoot)
                {
                    try
                    {
                        BaseClient[] list = new BaseClient[_clients.Keys.Count];
                        _clients.Keys.CopyTo(list, 0);

                        foreach (BaseClient client in list)
                        {
                            client.Disconnect();
                        }

                        log.Debug("Stopping server! - Cleaning up client list!");
                    }
                    catch (Exception e)
                    {
                        log.Error("Stop", e);
                    }
                }
            }
            log.Debug("Stopping server! - End of method!");
        }

        /// <summary>
        /// Remove a client from collection.
        /// </summary>
        /// <param name="client"></param>
        public virtual void RemoveClient(BaseClient client)
        {
            lock (_clients.SyncRoot)
            {
                _clients.Remove(client);
            }
        }

        public BaseClient[] GetAllClients()
        {
            lock (_clients.SyncRoot)
            {
                BaseClient[] temp = new BaseClient[_clients.Count];
                _clients.Keys.CopyTo(temp,0);
                return temp;
            }
        }

        public void Dispose()
        {
            ac_event.Dispose();
        }
    }
}
