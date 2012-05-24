using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using log4net;
using System.Reflection;
using System.Security.Cryptography;
using Game.Base.Packets;
using Game.Logic;
using Fighting.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Fighting.Server.Rooms;
using Game.Logic.Protocol;

namespace Fighting.Server
{
    public class ServerClient : BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private RSACryptoServiceProvider _rsa;

        private FightServer _svr;

        protected override void OnConnect()
        {
            base.OnConnect();
            _rsa = new RSACryptoServiceProvider();
            RSAParameters para = _rsa.ExportParameters(false);
            SendRSAKey(para.Modulus, para.Exponent);
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();

            _rsa = null;
        }

        #region 处理协议

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            switch (pkg.Code)
            {
                case (int)eFightPackageType.LOGIN:
                    HandleLogin(pkg);
                    break;
                //case (int)eFightPackageType.IP_PORT:
                //    HandleIPAndPort(pkg);
                    //break;
                case (int)eFightPackageType.GAME_ROOM_CREATE:
                    HandleGameRoomCreate(pkg);
                    break;
                case (int)eFightPackageType.GAME_PAIRUP_CANCEL:
                    HandleGameRoomCancel(pkg);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 战斗服务器登陆
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleLogin(GSPacketIn pkg)
        {
            byte[] rgb = pkg.ReadBytes();

            string[] temp = Encoding.UTF8.GetString(_rsa.Decrypt(rgb, false)).Split(',');

            if (temp.Length == 2)
            {
                _rsa = null;
                int id = int.Parse(temp[0]);
                //Info = ServerMgr.GetServerInfo(id);
                Strict = false;
            }
            else
            {
                log.ErrorFormat("Error Login Packet from {0}", TcpEndpoint);
                Disconnect();
            }
        }

        public void HandleIPAndPort(GSPacketIn pkg)
        {
            //IPAddress ip = new IPAddress((long)pkg.ReadDouble());
            //int port = pkg.ReadInt();
            //if (Info != null)
            //{
            //    Info.IP = ip.ToString();
            //    Info.Port = port;
            //}
        }

        public void HandleGameRoomCreate(GSPacketIn pkg)
        {
            int roomId = pkg.ReadInt();
            int gameType = pkg.ReadInt();
            int count = pkg.ReadInt();

            IGamePlayer[] players = new IGamePlayer[count];
            for (int i = 0; i < count; i++)
            {
                PlayerInfo info = new PlayerInfo();
                info.ID = pkg.ReadInt();
                info.Attack = pkg.ReadInt();
                info.Defence = pkg.ReadInt();
                info.Agility = pkg.ReadInt();
                info.Luck = pkg.ReadInt();

                double baseAttack = pkg.ReadDouble();
                double baseDefence = pkg.ReadDouble();
                double baseAgility = pkg.ReadDouble();
                double baseBlood = pkg.ReadDouble();
                int templateId = pkg.ReadInt();
                
                ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(templateId);
                ItemInfo item = new ItemInfo(itemTemplate);

                players[i] = new ProxyPlayer(info, item, baseAttack, baseDefence, baseAgility, baseBlood);
            }

            ProxyRoomMgr.CreateRoom(players, roomId, this);
        }

        public void HandleGameRoomCancel(GSPacketIn pkg)
        {
            int roomId = pkg.Parameter1;
            ProxyRoomMgr.CancelRoom(roomId);
        }

        #endregion

        #region 发送协议

        /// <summary>
        /// 发送RSA密钥
        /// </summary>
        /// <param name="m"></param>
        /// <param name="e"></param>
        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.RSAKey);
            pkg.Write(m);
            pkg.Write(e);
            SendTCP(pkg);
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServerClient(FightServer svr)
            : base(new byte[2048], new byte[2048])
        {
            _svr = svr;
        }

    }
}
