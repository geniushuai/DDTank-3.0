using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using System.Net;
using Game.Server.GameObjects;
using log4net;
using Game.Base.Packets;
using Game.Server.Managers;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using System.Security.Cryptography;
using System.Configuration;
using System.IO;
using Game.Server.GameUtils;

namespace Game.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class GameClient:BaseClient
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly byte[] POLICY = Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><!DOCTYPE cross-domain-policy SYSTEM \"http://www.adobe.com/xml/dtds/cross-domain-policy.dtd\"><cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" /></cross-domain-policy>\0");
        
        /// <summary>
        /// 用户实例
        /// </summary>
        protected GamePlayer m_player;

        public int Version;

        public GamePlayer Player
        {
            get { return m_player; }
            set
            {
                GamePlayer oldPlayer = Interlocked.Exchange(ref m_player, value);
                if (oldPlayer != null)
                {
                    oldPlayer.Quit();
                }
            }
        }

        /// <summary>
        /// 客户端最后发包时间
        /// </summary>
        protected long m_pingTime = DateTime.Now.Ticks;

        public long PingTime
        {
            get { return m_pingTime; }
        }

        /// <summary>
        /// 发送接口
        /// </summary>
        protected IPacketLib m_packetLib;

        public IPacketLib Out
        {
            get { return m_packetLib; }
            set { m_packetLib = value; }
        }

        /// <summary>
        /// TCP包处理器
        /// </summary>
        protected PacketProcessor m_packetProcessor;

        public PacketProcessor PacketProcessor
        {
            get { return m_packetProcessor; }
        }


        protected GameServer _srvr;

        public GameServer Server
        {
            get { return _srvr; }
        }

        /// <summary>
        /// 接收到客户端数据
        /// </summary>
        /// <param name="num_bytes"></param>
        public override void OnRecv(int num_bytes)
        {

            if(m_packetProcessor != null)
            {
                base.OnRecv(num_bytes);
            } 
            else if ( m_readBuffer[0] == '<')
            {
                m_sock.Send(POLICY);
            }
            else
            {
                base.OnRecv(num_bytes);
            }
            m_pingTime = DateTime.Now.Ticks;
        }

        /// <summary>
        /// 收到协议包
        /// </summary>
        /// <param name="pkg"></param>
        public override void OnRecvPacket(GSPacketIn pkg)
        {
            if (m_packetProcessor == null)
            {
                m_packetLib = AbstractPacketLib.CreatePacketLibForVersion(1, this);
                m_packetProcessor = new PacketProcessor(this);
            }
            if (m_player != null)
            {
                pkg.ClientID = m_player.PlayerId;
                pkg.WriteHeader();
            }
            //LogMsg(Marshal.ToHexDump("recevie:", pkg.Buffer, 0, pkg.Length));
            m_packetProcessor.HandlePacket(pkg);
        }

        public override void SendTCP(GSPacketIn pkg)
        {
            base.SendTCP(pkg);
            //LogMsg(Marshal.ToHexDump("sent", pkg.Buffer, 0, pkg.Length));
        }

        public override void DisplayMessage(string msg)
        {
            base.DisplayMessage(msg);
        }

        //private StreamWriter m_writer;
        //private static volatile int count;
        //public void LogMsg(string msg)
        //{
        //    lock (m_writer)
        //    {
        //        if (m_writer.BaseStream != null && m_writer.BaseStream.CanWrite)
        //            m_writer.WriteLine("{0}:{1} {2}", m_player, DateTime.Now, msg);
        //    }
        //}
        /// <summary>
        /// 客户端连接上
        /// </summary>
        protected override void OnConnect()
        {
            base.OnConnect();
            m_pingTime = DateTime.Now.Ticks;
            //if (Socket.RemoteEndPoint.AddressFamily == Socket.AddressFamily)
            //{
 
            //}
            //m_writer = new StreamWriter(File.Create(string.Format("./clients/{0}.log", count++)));
        }

        public override void Disconnect()
        {
            base.Disconnect();
           // LogMsg("disconneted!");
           // m_writer.Dispose();
        }
        private void resetStoreBag2(GamePlayer player)
        {

            PlayerInventory m_storeBag = player.StoreBag2;
            PlayerEquipInventory m_mainBag = player.MainBag;
            PlayerInventory m_propBag = player.PropBag;

            for (int i = 0; i < m_storeBag.Capalility; i++)
            {
                if (m_storeBag.GetItemAt(i) != null)
                {
                    var item = m_storeBag.GetItemAt(i);
                    if (item.Template.CategoryID == 10 || item.Template.CategoryID == 11 || item.Template.CategoryID == 12)
                    {
                        m_storeBag.MoveToStore(m_storeBag, i, m_propBag.FindFirstEmptySlot(1), m_propBag, 999);
                    }
                    else m_storeBag.MoveToStore(m_storeBag, i, m_mainBag.FindFirstEmptySlot(32), m_mainBag, 999);
                }

            }
        }
        /// <summary>
        /// 客户端断开连接
        /// </summary>
        protected override void OnDisconnect()
        {
            try
            {
                GamePlayer player = Interlocked.Exchange<GamePlayer>(ref m_player, null);
                if (player != null)
                {
                    player.FightBag.ClearBag();
                    resetStoreBag2(player);
                    LoginMgr.ClearLoginPlayer(player.PlayerCharacter.ID, this);
                    player.Quit();
                }

                byte[] temp = m_sendBuffer;
                m_sendBuffer = null;
                _srvr.ReleasePacketBuffer(temp);

                temp = m_readBuffer;
                m_readBuffer = null;
                _srvr.ReleasePacketBuffer(temp);
                
                base.OnDisconnect();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("OnDisconnect", e);
            }
        }

        /// <summary>
        /// 保存用户数据到数据库
        /// </summary>
        public void SavePlayer()
        {
            try
            {
                if (m_player != null)
                {
                    m_player.SaveIntoDatabase();
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SavePlayer", e);
            }
        }

        /// <summary>
        /// Constructor for a game client
        /// </summary>
        /// <param name="srvr">The server that's communicating with this client</param>
        public GameClient(GameServer svr,byte[] read,byte[] send)
            : base(read,send)
        {
            _srvr = svr;
            m_player = null;
            Encryted = true;
            AsyncPostSend = true;
        }

       

        /// <summary>
        /// Returns short informations about the client
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new StringBuilder(128)
                .Append(" pakLib:").Append(Out == null ? "(null)" : Out.GetType().FullName)
                .Append(" IP:").Append(TcpEndpoint)
                .Append(" char:").Append(Player == null ? "null" : Player.PlayerCharacter.NickName)
                .ToString();
        }
    }
}
