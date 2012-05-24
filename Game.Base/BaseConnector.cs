using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using log4net;
using System.Threading;
using Game.Base;

namespace Game.Base
{
    public class BaseConnector:BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly int RECONNECT_INTERVAL  = 1000 * 10 ; //ms;

        /// <summary>
        /// 接受回调事件参数
        /// </summary>
        private SocketAsyncEventArgs e;

        /// <summary>
        /// 连接的远程端口
        /// </summary>
        private IPEndPoint _remoteEP;

        public IPEndPoint RemoteEP
        {
            get
            {
                return _remoteEP;
            }
        }

        /// <summary>
        /// 是否自动重连
        /// </summary>
        private bool _autoReconnect;

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_readBufEnd = 0;
                m_sock.Connect(_remoteEP);
                log.InfoFormat("Connected to {0}", _remoteEP);
            }
            catch
            {
                log.ErrorFormat("Connect {0} failed!", _remoteEP);
                m_sock = null;
                return false;
            }
            OnConnect();
            ReceiveAsync();
            return true;
        }

        /// <summary>
        /// 重新连接的Timer调度
        /// </summary>
        private Timer timer;

        /// <summary>
        /// 尝试重新连接
        /// </summary>
        private void TryReconnect()
        {
            if (Connect())
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

                ReceiveAsync();
            }
            else
            {
                log.ErrorFormat("Reconnect {0} failed:", _remoteEP);
                log.ErrorFormat("Retry after {0} ms!", RECONNECT_INTERVAL);
                if (timer == null)
                    timer = new Timer(new TimerCallback(RetryTimerCallBack),this,Timeout.Infinite,Timeout.Infinite);
                timer.Change(RECONNECT_INTERVAL, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 重新连接的定时回调函数
        /// </summary>
        /// <param name="target"></param>
        private static void RetryTimerCallBack(Object target)
        {
            BaseConnector connector = target as BaseConnector;
            if (connector != null)
            {
                connector.TryReconnect();
            }
            else
            {
                log.Error("BaseConnector retryconnect timer return NULL!");
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public BaseConnector(string ip,int port,bool autoReconnect,byte[] readBuffer,byte[] sendBuffer):base(readBuffer,sendBuffer)
        {
            
            _remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            _autoReconnect = autoReconnect;
            e = new SocketAsyncEventArgs();
        }
    }
}
