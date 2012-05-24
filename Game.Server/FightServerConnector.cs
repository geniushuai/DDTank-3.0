using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using log4net;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System.Security.Cryptography;
using Game.Server.Managers;
using System.Threading;
using Game.Logic.Protocol;
using Game.Server.Rooms;

namespace Game.Server
{
    public class FightServerConnector : BaseConnector
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _loginKey;

        #region 重载函数
        protected override void OnDisconnect()
        {
            log.Error("FightServerConnector is OnDisconnect!");
            base.OnDisconnect();
        }
        #endregion

        #region 接受处理

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(AsynProcessPacket), pkg);
        }

        protected void AsynProcessPacket(object state)
        {
            try
            {
                GSPacketIn pkg = state as GSPacketIn;
                switch (pkg.Code)
                {
                    case (int)eFightPackageType.RSAKey:
                        HandleRSAKey(pkg);
                        break;
                    case (int)eFightPackageType.GAME_ROOM_CREATE:
                        HandleGameRoomCreate(pkg);
                        break;
                }
            }
            catch (Exception ex)
            {
                GameServer.log.Error("AsynProcessPacket", ex);
            }
        }

        protected void HandleRSAKey(GSPacketIn packet)
        {
            RSAParameters para = new RSAParameters();
            para.Modulus = packet.ReadBytes(128);
            para.Exponent = packet.ReadBytes();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(para);
            SendRSALogin(rsa, _loginKey);
        }

        protected void HandleGameRoomCreate(GSPacketIn packet)
        {

        }

        #endregion

        #region 发送协议

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="name"></param>
        public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.LOGIN);
            pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
            SendTCP(pkg);
        }

        public void SendPingCenter()
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PING);
            SendTCP(pkg);
        }

        /// <summary>
        /// 转发包
        /// </summary>
        /// <param name="packet"></param>
        public void SendTCP(GSPacketIn packet, BaseRoom room)
        {
            packet.Parameter1 = room.RoomId;
            SendTCP(packet);
        }


        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="loginKey"></param>
        public FightServerConnector(string ip, int port, int serverid, string name)
            : base(ip, port, true, new byte[2048], new byte[2048])
        {
            //_loginKey = string.Format("{0},{1}",serverid,name);
        }

        #endregion

        internal void SendRemoveRoom(Game.Server.Rooms.BaseRoom room)
        {
            GSPacketIn pkg = new GSPacketIn((int)eFightPackageType.GAME_PAIRUP_CANCEL);
            SendTCP(pkg, room);
        }

        internal void SendAddRoom(Game.Server.Rooms.BaseRoom room)
        {
            GSPacketIn pkg = new GSPacketIn((int)eFightPackageType.GAME_ROOM_CREATE);
            pkg.WriteInt(room.RoomId);
            pkg.WriteInt((int)room.GameType);
            pkg.WriteInt(room.Game.PlayerCount);

            List<GamePlayer> players = room.GetPlayersSafe();
            foreach (GamePlayer p in players)
            {
                pkg.WriteInt(p.PlayerCharacter.ID);//改为唯一ID
                pkg.WriteInt(p.PlayerCharacter.Attack);
                pkg.WriteInt(p.PlayerCharacter.Defence);
                pkg.WriteInt(p.PlayerCharacter.Agility);
                pkg.WriteInt(p.PlayerCharacter.Luck);
                pkg.WriteDouble(p.GetBaseAttack());
                pkg.WriteDouble(p.GetBaseDefence());
                pkg.WriteDouble(p.GetBaseAgility());
                pkg.WriteDouble(p.GetBaseBlood());
                pkg.WriteInt(p.CurrentWeapon.TemplateID);
            }
            SendTCP(pkg, room);
        }

    }
}
