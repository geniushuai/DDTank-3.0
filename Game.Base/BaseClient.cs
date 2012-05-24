using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Net.Sockets;
using System.Collections;
using Game.Base.Packets;
using System.Threading;

namespace Game.Base
{
    public delegate void ClientEventHandle(BaseClient client);

    public class BaseClient
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Socket that holds the client connection
        /// </summary>
        protected Socket m_sock;

        /// <summary>
        /// Packet buffer, holds incoming packet data
        /// </summary>
        protected byte[] m_readBuffer;

        /// <summary>
        /// Current offset into the buffer
        /// </summary>
        protected int m_readBufEnd;

        private SocketAsyncEventArgs rc_event;

        /// <summary>
        /// Gets or sets the socket the client is using
        /// </summary>
        public Socket Socket
        {
            get
            {
                return m_sock;
            }
            set
            {
                m_sock = value;
            }
        }

        /// <summary>
        /// Gets the packet buffer for the client
        /// </summary>
        public byte[] PacketBuf
        {
            get
            {
                return m_readBuffer;
            }
        }

        /// <summary>
        /// The remote client is connected
        /// </summary>
        private int m_isConnected;
        public bool IsClientPacketSended = true;
        public int numPacketProcces = 0;
        public bool IsConnected
        {
            get { return m_isConnected == 1; }
        }

        /// <summary>
        /// Gets or sets the offset into the receive buffer
        /// </summary>
        public int PacketBufSize
        {
            get { return m_readBufEnd; }
            set { m_readBufEnd = value; }
        }

        /// <summary>
        /// Gets the client's TCP endpoint string, if connected
        /// </summary>
        public string TcpEndpoint
        {
            get
            {
                Socket s = m_sock;
                if (s != null && s.Connected && s.RemoteEndPoint != null)
                    return s.RemoteEndPoint.ToString();
                return "not connected";
            }
        }

        /// <summary>
        /// Send buffer
        /// </summary>
        protected byte[] m_sendBuffer;

        public byte[] SendBuffer
        {
            get
            {
                return m_sendBuffer;
            }
        }

        private bool m_encryted;
        private static short index = 0;
        public bool Encryted
        {
            get
            {
                return m_encryted;
            }
            set
            {
                m_encryted = value;
            }
        }

        private bool m_strict;
        public bool Strict
        {
            get { return m_strict; }
            set { m_strict = value; }
        }

        private bool m_asyncPostSend;

        public bool AsyncPostSend
        {
            get { return m_asyncPostSend; }
            set { m_asyncPostSend = value; }
        }

        /// <summary>
        /// Stream buffer processor.
        /// </summary>
        public StreamProcessor m_processor;

        public event ClientEventHandle Connected;

        public event ClientEventHandle Disconnected;

        /// <summary>
        /// Called when data has been received from the connection
        /// </summary>
        /// <param name="num_bytes">Number of bytes received in m_pbuf</param>
        public virtual void OnRecv(int num_bytes) 
        {
            m_processor.ReceiveBytes(num_bytes);
        }

        /// <summary>
        /// Called wehn pkg has been dispath from the received data
        /// </summary>
        /// <param name="pkg"></param>
        public virtual void OnRecvPacket(GSPacketIn pkg) { }

        /// <summary>
        /// Called after the client connection has been accepted
        /// </summary>
        protected virtual void OnConnect() 
        {
            int connected = Interlocked.Exchange( ref m_isConnected, 1);
            if (connected == 0 && Connected != null)
            {
                Connected(this);
            }
        }

        /// <summary>
        /// Called right after the client has been disconnected
        /// </summary>
        protected virtual void OnDisconnect() 
        {
            if (Disconnected != null)
            {
                Disconnected(this);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="srvr">Pointer to the server the client is connected to</param>
        public BaseClient(byte[] readBuffer,byte[] sendBuffer)
        {
            m_readBuffer = readBuffer;
            m_sendBuffer = sendBuffer;
            m_readBufEnd = 0;
            rc_event = new SocketAsyncEventArgs();
            rc_event.Completed += RecvEventCallback;
            m_processor = new StreamProcessor(this);
            m_encryted = false;
            m_strict = true;
        }

        public void SetFsm(int adder, int muliter)
        {
            m_processor.SetFsm(adder, muliter);
        }

        /// <summary>
        /// Bengin a asynchnorous receive data call.It will add callback function to e.Completed event.
        /// </summary>
        /// <param name="e"></param>
        public void ReceiveAsync()
        {
            ReceiveAsyncImp(rc_event);
        }

        /// <summary>
        /// Implements a asynchnorous receive data call.
        /// </summary>
        /// <param name="e"></param>
        private void ReceiveAsyncImp(SocketAsyncEventArgs e)
        {
            if (m_sock != null && m_sock.Connected)
            {
                int bufSize = m_readBuffer.Length;
                if (m_readBufEnd >= bufSize) //Do we have space to receive?
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error(TcpEndpoint + " disconnected because of buffer overflow!");
                        log.Error("m_pBufEnd=" + m_readBufEnd + "; buf size=" + bufSize);
                        log.Error(m_readBuffer);
                    }
                    Disconnect();
                }
                else      
                {
                    e.SetBuffer(m_readBuffer, m_readBufEnd, bufSize - m_readBufEnd);
                    if (!m_sock.ReceiveAsync(e))
                    {
                        //log.Error("ReceiveAsyncImp:RecvEventCallback:" + TcpEndpoint);
                        RecvEventCallback(m_sock, e);
                    }
                    //TrieuLSL
                    //m_sock.Send(e.Buffer);
                }
            }
            else
            {
                Disconnect();
            }
        }

        /// <summary>
        /// SocketAsyncEvent.Completed event callback function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecvEventCallback(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                int num_bytes = e.BytesTransferred;
                
                if (num_bytes >0)
                {
                    this.OnRecv(num_bytes);
                    this.ReceiveAsyncImp(e);
                }
                else
                {
                    log.InfoFormat("Disconnecting client ({0}), received bytes={1}", TcpEndpoint, num_bytes);
                    Disconnect();
                }
            }
            catch (Exception ex)
            {   
                log.ErrorFormat("{0} RecvCallback:{1}", TcpEndpoint,ex);
                Disconnect();
            }
        }

        public virtual void SendTCP(GSPacketIn pkg)
        {
            m_processor.SendTCP(pkg);
        }

        public bool SendAsync(SocketAsyncEventArgs e)
        {
            int start = Environment.TickCount;
            log.Debug(string.Format("Send To ({0}) {1} bytes", TcpEndpoint, e.Count));
            
            bool result = true;
            if (m_sock.Connected)
            {

                //TrieuLSL suu hom nay
                result = m_sock.SendAsync(e);
              // m_sock.Send(e.Buffer);
               // result = true;
            }

            int took = Environment.TickCount - start;
            if (took > 100)
                log.WarnFormat("AsyncTcpSendCallback.BeginSend took {0}ms! (TCP to client: {1})", took,TcpEndpoint);

            return result;
        }


        /// <summary>
        /// Closes the client connection
        /// </summary>
        protected void CloseConnections()
        {
            if (m_sock != null)
            {
                try { m_sock.Shutdown(SocketShutdown.Both); }
                catch { }
                try { m_sock.Close(); }
                catch { }
            }
        }

        public virtual bool Connect(Socket connectedSocket)
        {
            m_sock = connectedSocket;
            if (m_sock.Connected)
            {
                int connected = Interlocked.Exchange(ref m_isConnected, 1);
                if (connected == 0)
                {
                    OnConnect();
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Closes the client connection
        /// </summary>
        public virtual void Disconnect()
        {
            try
            {
                int connected = Interlocked.Exchange(ref m_isConnected, 0);
                if (connected == 1)
                {
                    CloseConnections();
                    OnDisconnect();
                    rc_event.Dispose();
                    m_processor.Dispose();
                }
                
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Exception", e);
            }
        }

        public virtual void DisplayMessage(string msg) { }


        public  byte[] RECEIVE_KEY;
        public byte[] SEND_KEY;
        public virtual void resetKey()
        {

            RECEIVE_KEY= StreamProcessor.cloneArrary(StreamProcessor.KEY);
            SEND_KEY = StreamProcessor.cloneArrary(StreamProcessor.KEY);
        }


        public virtual void setKey(byte[] data)
        {

            for (int i = 0; i < 8; i++)
            {
                RECEIVE_KEY[i] = data[i];
                SEND_KEY[i] = data[i];
            }
        }
    }
}
